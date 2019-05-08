using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Helpers;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Services.Interfaces.ProviderService;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using Dfc.CourseDirectory.Services.ProviderService;
using Dfc.CourseDirectory.Services.VenueService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configuration 

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient((provider) => new HttpClient())
                .Configure<VenueServiceSettings>(venueServiceSettingsOptions =>
                {
                    venueServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("VenueServiceSettings:ApiUrl");
                    venueServiceSettingsOptions.ApiKey = configuration.GetValue<string>("VenueServiceSettings:ApiKey");
                }
                )
                .AddScoped<IVenueService, VenueService>()
                .Configure<ProviderServiceSettings>(providerServiceSettingsOptions =>
                {
                    providerServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("ProviderServiceSettings:ApiUrl");
                    providerServiceSettingsOptions.ApiKey = configuration.GetValue<string>("ProviderServiceSettings:ApiKey");
                }
                )
                .AddScoped<IProviderService, ProviderService>()              
                .BuildServiceProvider();

            // Configure console logging
            serviceProvider
                .GetService<ILoggerFactory>();
            //.AddConsole(LogLevel.Debug); // Not Working and not needed

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting Dfc.CourseDirectory.ApprenticeshipMigrationTool application");

            // Initialise the services
            var venueService = serviceProvider.GetService<IVenueService>();
            var providerService = serviceProvider.GetService<IProviderService>();

            logger.LogDebug("Log test.");


            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool automatedMode = configuration.GetValue<bool>("AutomatedMode");
            bool generateJsonFilesLocally = configuration.GetValue<bool>("GenerateJsonFilesLocally");
            bool generateReportFilesLocally = configuration.GetValue<bool>("GenerateReportFilesLocally");
            string jsonCourseFilesPath = configuration.GetValue<string>("JsonCourseFilesPath");
            string selectionOfProvidersFileName = configuration.GetValue<string>("SelectionOfProvidersFileName");
            DeploymentEnvironment deploymentEnvironment = configuration.GetValue<DeploymentEnvironment>("DeploymentEnvironment");
            //TransferMethod transferMethod = configuration.GetValue<TransferMethod>("TransferMethod");


            #endregion

            //Console.WriteLine("Hello World!");
            //string providerInput = Console.ReadLine();

            #region Get User Input and Set Variables

            string adminReport = "                         Admin Report " + Environment.NewLine;
            adminReport += "________________________________________________________________________________" + Environment.NewLine + Environment.NewLine;

            var providerUKPRNList = new List<int>();
            int courseTransferId = 0;
            bool goodToTransfer = false;
            TransferMethod transferMethod = TransferMethod.Undefined;
            int? singleProviderUKPRN = null;
            string bulkUploadFileName = string.Empty;

            if (automatedMode)
            {
                Console.WriteLine("The Migration Apprenticeship Tool is running in Automated Mode." + Environment.NewLine + "Please, do not close this window until \"Migration completed\" message is displayed." + Environment.NewLine);

                string errorMessageGetCourses = string.Empty;
                providerUKPRNList = DataHelper.GetProviderUKPRNs(connectionString, out errorMessageGetCourses);
                if (!string.IsNullOrEmpty(errorMessageGetCourses))
                {
                    adminReport += errorMessageGetCourses + Environment.NewLine;
                }
                else
                {
                    goodToTransfer = true;
                    transferMethod = TransferMethod.CourseMigrationTool;
                }
            }
            else
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine(" APPRENTICESHIPS MIGRATION");
                Console.WriteLine("---------------------------");
                Console.WriteLine("Please enter valid UKPRN to migrate apprenticeships for a single Provider" + Environment.NewLine + "or \"s\" to migrate apprenticeships for a selection of Providers:" +
                                   Environment.NewLine + "(Ensure that you have created a folder named 'ProviderSelections' and placed your .csv file in it.)");
                string providerInput = Console.ReadLine();

                if (string.IsNullOrEmpty(providerInput))
                {
                    Console.WriteLine("Please next time enter a value.");
                }
                else if (providerInput.Equals("s", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Migrate selection of Providers from .CSV file
                    string ProviderSelectionsPath = string.Format(@"{0}\ProviderSelections", jsonCourseFilesPath);
                    if (!Directory.Exists(ProviderSelectionsPath))
                        Directory.CreateDirectory(ProviderSelectionsPath);
                    string selectionOfProviderFile = string.Format(@"{0}\{1}", ProviderSelectionsPath, selectionOfProvidersFileName);
                    using (StreamReader reader = new StreamReader(selectionOfProviderFile))
                    {
                        string line = null;
                        while (null != (line = reader.ReadLine()))
                        {
                            string[] providers = line.Split(',');
                            foreach (var provider in providers)
                            {
                                if (CheckForValidUKPRN(provider))
                                {
                                    providerUKPRNList.Add(Convert.ToInt32(provider));

                                    goodToTransfer = true;
                                    bulkUploadFileName = selectionOfProvidersFileName;
                                    transferMethod = TransferMethod.CourseMigrationToolCsvFile;
                                }
                                else if (string.IsNullOrEmpty(provider))
                                {
                                    // We don't want to know about the empty spaces on splitting
                                }
                                else
                                {
                                    // Log invalid providers
                                    adminReport += $">>> The following ( { provider } ) is not valid UKPRN." + Environment.NewLine + Environment.NewLine;
                                }
                            }
                        }
                    }


                }
                else if (CheckForValidUKPRN(providerInput))
                {
                    // Migrate single Provider
                    providerUKPRNList.Add(Convert.ToInt32(providerInput));
                    goodToTransfer = true;
                    transferMethod = TransferMethod.CourseMigrationToolSingleUkprn;
                    singleProviderUKPRN = Convert.ToInt32(providerInput);
                }
                else
                {
                    Console.WriteLine("You have to enter either valid UKPRN (which must be 8 digit number starting with a 1 e.g. 10000364) or \"s\" for selection of Providers");
                }
            }

            // Auditing to be decided
            //if (goodToTransfer)
            //{
            //    string errorMessageCourseTransferAdd = string.Empty;
            //    if (providerUKPRNList != null && providerUKPRNList.Count > 0)
            //    {
            //        DataHelper.CourseTransferAdd(connectionString,
            //                                        DateTime.Now,
            //                                        (int)transferMethod,
            //                                        (int)deploymentEnvironment,
            //                                        string.Empty,
            //                                        "DFC – Course Migration Tool",
            //                                        singleProviderUKPRN,
            //                                        out errorMessageCourseTransferAdd,
            //                                        out courseTransferId);
            //    }
            //    if (!string.IsNullOrEmpty(errorMessageCourseTransferAdd)) adminReport += errorMessageCourseTransferAdd + Environment.NewLine;

            //    if (courseTransferId.Equals(-1))
            //    {
            //        adminReport += $"We cannot get the BatchNumber (CourseTransferId), so migration will be terminated. Number of UKPRNs ( { providerUKPRNList?.Count } )" + Environment.NewLine;
            //        providerUKPRNList = null;
            //    }
            //}


            Stopwatch adminStopWatch = new Stopwatch();
            adminStopWatch.Start();
            Stopwatch provStopWatch = new Stopwatch();
            provStopWatch.Start();

            int CountProviders = 0;


            #endregion

            foreach (var providerUKPRN in providerUKPRNList)
            {
                Console.WriteLine(providerUKPRN);
            }


            Console.WriteLine("Migration of Apprenticeships completed.");
            string lastLine = Console.ReadLine();
        }

        internal static bool CheckForValidUKPRN(string ukprn)
        {
            string regex = "^[1][0-9]{7}$";
            var validUKPRN = Regex.Match(ukprn, regex, RegexOptions.IgnoreCase);

            return validUKPRN.Success;
        }
    }
}
