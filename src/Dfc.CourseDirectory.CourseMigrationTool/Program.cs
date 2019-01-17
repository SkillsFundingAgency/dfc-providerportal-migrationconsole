using Dfc.CourseDirectory.CourseMigrationTool.Helpers;
using Dfc.CourseDirectory.CourseMigrationTool.Models;
using Dfc.CourseDirectory.Models.Models.Venues;
using Dfc.CourseDirectory.Services;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.Interfaces;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using Dfc.CourseDirectory.Services.VenueService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dfc.CourseDirectory.CourseMigrationTool
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

            //var venueServiceSettings = configuration.GetSection(nameof(VenueServiceSettings)); // DoesNotTakeAnything
            //VenueServiceSettings venueServiceSettings = new VenueServiceSettings
            //{
            //    ApiUrl = configuration.GetValue<string>("VenueServiceSettings:ApiUrl"), 
            //    ApiKey = configuration.GetValue<string>("VenueServiceSettings:ApiKey") 
            //};

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient((provider) => new HttpClient())
                .AddSingleton<IVenueService, VenueService>()
                //.Configure<VenueServiceSettings>(configuration.GetSection(nameof(VenueServiceSettings))) // Not Working
                .Configure<VenueServiceSettings>(venueServiceSettingsOptions =>
                    {
                        venueServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("VenueServiceSettings:ApiUrl");
                        venueServiceSettingsOptions.ApiKey = configuration.GetValue<string>("VenueServiceSettings:ApiKey");
                    }
                )
                .AddSingleton<ICourseService, CourseService>()
                 .Configure<LarsSearchSettings>(larsSearchSettingsOptions =>
                 {
                     larsSearchSettingsOptions.ApiUrl = configuration.GetValue<string>("LarsSearchSettings:ApiUrl");
                     larsSearchSettingsOptions.ApiKey = configuration.GetValue<string>("LarsSearchSettings:ApiKey");
                     larsSearchSettingsOptions.ApiVersion = configuration.GetValue<string>("LarsSearchSettings:ApiVersion");
                     larsSearchSettingsOptions.Indexes = configuration.GetValue<string>("LarsSearchSettings:Indexes");
                     larsSearchSettingsOptions.ItemsPerPage = Convert.ToInt32(configuration.GetValue<string>("LarsSearchSettings:ItemsPerPage"));
                     larsSearchSettingsOptions.PageParamName = configuration.GetValue<string>("LarsSearchSettings:PageParamName");
                 }
                )
                .AddSingleton<ILarsSearchService, LarsSearchService>()
                .BuildServiceProvider();

            // Configure console logging
            serviceProvider
                .GetService<ILoggerFactory>();
            //.AddConsole(LogLevel.Debug); // Not Working and not needed

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            // Initialise the services
            var venueService = serviceProvider.GetService<IVenueService>();
            var larsSearchService = serviceProvider.GetService<ILarsSearchService>();

            logger.LogDebug("Log test.");


            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool generateFilesLocally = configuration.GetValue<bool>("GenerateFilesLocally");
            string jsonCourseFilesPath = configuration.GetValue<string>("JsonCourseFilesPath");

            #endregion 

            Console.WriteLine("Please enter UKPRN:");
            string UKPRN = Console.ReadLine();

            if (!CheckForValidUKPRN(UKPRN))
            {
                Console.WriteLine("UKPRN must be 8 digit number starting with a 1 e.g. 10000364");
                UKPRN = Console.ReadLine();
            }

            int providerUKPRN = Convert.ToInt32(UKPRN);

            string providerName = string.Empty;
            bool advancedLearnerLoan = false;
            var tribalCourses = DataHelper.GetCoursesByProviderUKPRN(providerUKPRN, connectionString, out providerName, out advancedLearnerLoan);

            foreach (var tribalCourse in tribalCourses)
            {
                // DO NOT MIGRATE COURSES WITHOUT A LARS REFERENCE. WE WILL LOOK TO AUGMENT THIS DATA WITH AN ILR EXTRACT
                if (string.IsNullOrEmpty(tribalCourse.LearningAimRefId))
                {
                    // DO not migrate => log the course
                }
                else
                {
                    // IF QualificationType is missing, get it from LarsSearchService
                    //if (string.IsNullOrEmpty(tribalCourse.Qualification))
                    //{
                    LarsSearchCriteria criteria = new LarsSearchCriteria(tribalCourse.LearningAimRefId, 10, 0, string.Empty, null);
                    var larsResult = Task.Run(async () => await larsSearchService.SearchAsync(criteria)).Result;

                    if (larsResult.IsSuccess && larsResult.HasValue)
                    {
                        var qualifications = new List<string>();
                        foreach (var item in larsResult.Value.Value)
                        {
                            qualifications.Add(item.LearnAimRefTypeDesc);
                        }

                        if (qualifications.Count.Equals(0))
                        {
                            string logNoQual = "N ????? - ";
                        }
                        else if (qualifications.Count.Equals(1))
                        {
                            tribalCourse.Qualification = qualifications[0];
                        }
                        else
                        {
                            string logMoreQuals = "N ????? - ";
                        }
                    }
                    else
                    {
                        string logQualService = "Np Venue with this ????? - " + "" + ", Error: " + larsResult?.Error;
                    }
                    //}

                    tribalCourse.AdvancedLearnerLoan = advancedLearnerLoan;

                    var tribalCourseRuns = DataHelper.GetCourseInstancesByCourseId(tribalCourse.CourseId, connectionString);

                    if (tribalCourseRuns != null)
                    {
                        tribalCourse.TribalCourseRuns = tribalCourseRuns;
                        foreach (var tribalCourseRun in tribalCourse.TribalCourseRuns)
                        {
                            tribalCourseRun.CourseName = tribalCourse.CourseTitle;
                            // Call VenueService and for each tribalCourseRun.VenueId get tribalCourseRun.VenueGuidId
                            if (tribalCourseRun.VenueId != null)
                            {
                                GetVenueByVenueIdCriteria venueId = new GetVenueByVenueIdCriteria(tribalCourseRun.VenueId ?? 0); //{ venueId = tribalCourseRun.VenueId ?? 0 };
                                var venueResult = Task.Run(async () => await venueService.GetVenueByVenueIdAsync(venueId)).Result;

                                //string Id = "cfb5fac2-867b-48d9-91fc-679a9b019bd1";
                                //GetVenueByIdCriteria venueGuid2Id = new GetVenueByIdCriteria(Id);
                                //var result = Task.Run(async () => await venueService.GetVenueByIdAsync(venueGuid2Id)).Result;


                                if (venueResult.IsSuccess && venueResult.HasValue)
                                {
                                    tribalCourseRun.VenueGuidId = new Guid(((Venue)venueResult.Value).ID);
                                }
                                else
                                {
                                    string logNoVenue = "Np Venue with this VenueId - " + tribalCourseRun.VenueId + ", Error: " + venueResult?.Error;
                                }
                            }
                            else
                            {
                                string logNoVenueId = "No VenueId";
                            }
                        }
                    }

                    // Do the mapping
                    var course = MappingHelper.MapTribalCourseToCourse(tribalCourse);

                    // Send course via CourseService
                    if (generateFilesLocally)
                    {
                        var courseJson = JsonConvert.SerializeObject(course);
                        string jsonFileName = string.Format("{0}-{1}-{2}-{3}-{4}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), course.ProviderUKPRN, course.LearnAimRef, course.CourseId, tribalCourseRuns.Count.ToString());
                        File.WriteAllText(string.Format(@"{0}\{1}", jsonCourseFilesPath, jsonFileName), courseJson);
                    }
                    else
                    {
                        // Call the service
                    }
                }
            }

            Console.WriteLine(providerName);
            string nextLine = Console.ReadLine();

        }

        internal static bool CheckForValidUKPRN(string ukprn)
        {
            string regex = "^[1][0-9]{7}$";
            var validUKPRN = Regex.Match(ukprn, regex, RegexOptions.IgnoreCase);

            return validUKPRN.Success;
        }
    }
}
