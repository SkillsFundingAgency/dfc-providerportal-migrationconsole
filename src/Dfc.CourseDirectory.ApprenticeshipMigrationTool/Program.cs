using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Helpers;
using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Models;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Interfaces.Providers;
using Dfc.CourseDirectory.Models.Models.Apprenticeships;
using Dfc.CourseDirectory.Services.Interfaces.ProviderService;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using Dfc.CourseDirectory.Services.ProviderService;
using Dfc.CourseDirectory.Services.VenueService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Configuration 

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

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
            bool AutomatedMode = configuration.GetValue<bool>("AutomatedMode");
            bool GenerateJsonFilesLocally = configuration.GetValue<bool>("GenerateJsonFilesLocally");
            bool GenerateReportFilesLocally = configuration.GetValue<bool>("GenerateReportFilesLocally");
            string JsonApprenticeshipFilesPath = configuration.GetValue<string>("JsonApprenticeshipFilesPath");
            string selectionOfProvidersFileName = configuration.GetValue<string>("SelectionOfProvidersFileName");
            DeploymentEnvironment deploymentEnvironment = configuration.GetValue<DeploymentEnvironment>("DeploymentEnvironment");
            //TransferMethod transferMethod = configuration.GetValue<TransferMethod>("TransferMethod");

            bool UpdateProvider = configuration.GetValue<bool>("UpdateProvider");


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

            if (AutomatedMode)
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
                    string ProviderSelectionsPath = string.Format(@"{0}\ProviderSelections", JsonApprenticeshipFilesPath);
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
                Console.WriteLine("Provider - " + providerUKPRN);
                string providerReport = "                         Migration Report " + Environment.NewLine;
                // GetProviderDetailsByUKPRN
                string errorMessageGetProviderDetailsByUKPRN = string.Empty;
                var provider = DataHelper.GetProviderDetailsByUKPRN(providerUKPRN, connectionString, out errorMessageGetProviderDetailsByUKPRN);
                var ProviderGuidId = new Guid();
                if (!string.IsNullOrEmpty(errorMessageGetProviderDetailsByUKPRN))
                {
                    adminReport += errorMessageGetProviderDetailsByUKPRN + Environment.NewLine;
                }
                else
                {


                    var providerCriteria = new ProviderSearchCriteria(providerUKPRN.ToString());
                    var providerResult = Task.Run(async () => await providerService.GetProviderByPRNAsync(providerCriteria)).Result;

                    if (providerResult.IsSuccess && providerResult.HasValue)
                    {
                        var providers = providerResult.Value.Value;
                        if (providers.Count().Equals(1))
                        {
                            var providerToUpdate = providers.FirstOrDefault();
                            ProviderGuidId = providerToUpdate.id; // We need our Provider GUID id

                            #region  Update Provider

                            if (UpdateProvider)
                            {
                                providerToUpdate.ProviderName = provider.ProviderName;
                                providerToUpdate.TradingName = provider.TradingName;
                                providerToUpdate.ProviderId = provider.ProviderId;
                                providerToUpdate.UPIN = provider.UPIN;
                                providerToUpdate.MarketingInformation = provider.MarketingInformation;

                                if (!string.IsNullOrEmpty(provider.ProviderNameAlias))
                                {
                                    if (providerToUpdate.ProviderAliases != null && providerToUpdate.ProviderAliases[0].ProviderAlias == null)
                                    {
                                        var providerAlias = new Provideralias();
                                        providerAlias.ProviderAlias = provider.ProviderNameAlias;
                                        providerAlias.LastUpdated = DateTime.Now;
                                        providerToUpdate.ProviderAliases = new IProvideralias[] { providerAlias };
                                    }
                                }

                                var ApprenticeshipProviderContact = new Providercontact();
                                ApprenticeshipProviderContact.ContactType = "A";
                                ApprenticeshipProviderContact.ContactTelephone1 = provider.Telephone;
                                ApprenticeshipProviderContact.ContactEmail = provider.Email;
                                ApprenticeshipProviderContact.ContactWebsiteAddress = provider.Website;
                                ApprenticeshipProviderContact.LastUpdated = DateTime.Now;
                                if (providerToUpdate.ProviderContact == null)
                                {
                                    providerToUpdate.ProviderContact = new IProvidercontact[] { ApprenticeshipProviderContact };
                                }
                                else
                                {
                                    providerToUpdate.ProviderContact = providerToUpdate.ProviderContact.Append(ApprenticeshipProviderContact).ToArray();
                                }


                                if (GenerateJsonFilesLocally)
                                {
                                    var providerToUpdateJson = JsonConvert.SerializeObject(providerToUpdate);
                                    string jsonUpdateProviderPathFileName = string.Format("{0}-UpdateProvider-{1}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), providerToUpdate.UnitedKingdomProviderReferenceNumber);
                                    string UpdateProviderPath = string.Format(@"{0}\UpdateProvider", JsonApprenticeshipFilesPath);
                                    if (!Directory.Exists(UpdateProviderPath))
                                        Directory.CreateDirectory(UpdateProviderPath);
                                    File.WriteAllText(string.Format(@"{0}\{1}", UpdateProviderPath, jsonUpdateProviderPathFileName), providerToUpdateJson);
                                }
                                else
                                {
                                    // Call ProviderService API to update provider
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            providerReport += $"We CANNOT IDENTIFY the Provider - " + Environment.NewLine + Environment.NewLine;
                        }
                    }
                    else
                    {
                        providerReport += $"ERROR on GETTING the Provider - { providerResult.Error }" + Environment.NewLine + Environment.NewLine;
                    }

                    // Get Apprenticeships by ProviderId                    
                    string errorMessageGetApprenticeshipsByProviderId = string.Empty;
                    var apprenticeships = DataHelper.GetApprenticeshipsByProviderId(provider.ProviderId ?? 0, connectionString, out errorMessageGetApprenticeshipsByProviderId);
                    if (!string.IsNullOrEmpty(errorMessageGetApprenticeshipsByProviderId))
                    {
                        adminReport += errorMessageGetApprenticeshipsByProviderId + Environment.NewLine;
                    }
                    else
                    {
                        foreach (var apprenticeship in apprenticeships)
                        {
                            // // Mapp Apprenticeships
                            apprenticeship.id = Guid.NewGuid();
                            apprenticeship.ProviderId = ProviderGuidId;
                            apprenticeship.ProviderUKPRN = providerUKPRN;

                            apprenticeship.CreatedDate = DateTime.Now;
                            apprenticeship.CreatedBy = "DFC – Apprenticeship Migration Tool";

                            // Get Framework/Standard GUID id => ???? Call ReferenceData Service
                            if (apprenticeship.FrameworkCode != null && apprenticeship.ProgType != null && apprenticeship.PathwayCode != null)
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.FrameworkCode;
                            else if (apprenticeship.StandardCode != null && apprenticeship.Version != null)
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.StandardCode;
                            else
                                apprenticeship.ApprenticeshipType = ApprenticeshipType.Undefined;

                            // Get ApprenticeshipLocation
                            var apprenticeshipLocations = new List<ApprenticeshipLocation>();

                            // Get ApprenticeshipLocationDeliveryMode

                            // Get Location per 

                            // Checks locations / Add Locations

                            // Add Apprenticeship to CosmosDB
                            if (GenerateJsonFilesLocally)
                            {
                                var apprenticeshipJson = JsonConvert.SerializeObject(apprenticeship);
                                string jsonFileName = string.Format("{0}-Apprenticeship-{1}-{2}-{3}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), providerUKPRN, apprenticeship.ApprenticeshipId, apprenticeship?.ApprenticeshipLocations?.Count());
                                if (!Directory.Exists(JsonApprenticeshipFilesPath))
                                    Directory.CreateDirectory(JsonApprenticeshipFilesPath);
                                File.WriteAllText(string.Format(@"{0}\{1}", JsonApprenticeshipFilesPath, jsonFileName), apprenticeshipJson);
                            }
                            else
                            {
                                // Call ApprenticeshipService to Add a Apprenticeship
                            }
                        }
                    }
                }
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
