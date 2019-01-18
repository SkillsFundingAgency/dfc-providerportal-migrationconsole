using Dfc.CourseDirectory.CourseMigrationTool.Helpers;
using Dfc.CourseDirectory.CourseMigrationTool.Models;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Models.Courses;
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
using System.Diagnostics;
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
                //.AddSingleton<IVenueService, VenueService>()
                //.Configure<VenueServiceSettings>(configuration.GetSection(nameof(VenueServiceSettings))) // Not Working
                .Configure<VenueServiceSettings>(venueServiceSettingsOptions =>
                    {
                        venueServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("VenueServiceSettings:ApiUrl");
                        venueServiceSettingsOptions.ApiKey = configuration.GetValue<string>("VenueServiceSettings:ApiKey");
                    }
                )
                .AddScoped<IVenueService, VenueService>()
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
                .AddScoped<ILarsSearchService, LarsSearchService>()
                .Configure<CourseServiceSettings>(courseServiceSettingsOptions =>
                {
                    courseServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("CourseServiceSettings:ApiUrl");
                    courseServiceSettingsOptions.ApiKey = configuration.GetValue<string>("CourseServiceSettings:ApiKey");
                }
                )
                .AddScoped<ICourseService, CourseService>()
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
            var courseService = serviceProvider.GetService<ICourseService>();

            logger.LogDebug("Log test.");


            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool generateFilesLocally = configuration.GetValue<bool>("GenerateFilesLocally");
            string jsonCourseFilesPath = configuration.GetValue<string>("JsonCourseFilesPath");

            #endregion 

            string providerReport = "            Migration Report " + Environment.NewLine;
            int CountCourseFailures = 0;

            string adminReport = " Admin Report " + Environment.NewLine + Environment.NewLine;
            int CountProviders = 0;

            Console.WriteLine("Please enter UKPRN:");
            string UKPRN = Console.ReadLine();

            if (!CheckForValidUKPRN(UKPRN))
            {
                Console.WriteLine("UKPRN must be 8 digit number starting with a 1 e.g. 10000364");
                UKPRN = Console.ReadLine();
            }

            int providerUKPRN = Convert.ToInt32(UKPRN);

            Stopwatch adminStopWatch = new Stopwatch();
            adminStopWatch.Start();

            CountProviders = 1;

            Stopwatch providerStopWatch = new Stopwatch();
            providerStopWatch.Start();

            string providerName = string.Empty;
            bool advancedLearnerLoan = false;
            var tribalCourses = DataHelper.GetCoursesByProviderUKPRN(providerUKPRN, connectionString, out providerName, out advancedLearnerLoan);

            string reportForProvider = $"for Provider '{ providerName }' with UKPRN  ( { UKPRN } )";
            providerReport += reportForProvider + Environment.NewLine + Environment.NewLine;
            providerReport += $"Courses {tribalCourses.Count } to be migrated "  + Environment.NewLine;

            foreach (var tribalCourse in tribalCourses)
            {
                providerReport += Environment.NewLine + $">>> Course { tribalCourse.CourseId } LARS: { tribalCourse.LearningAimRefId } to be migrated " + Environment.NewLine;
                // DO NOT MIGRATE COURSES WITHOUT A LARS REFERENCE. WE WILL LOOK TO AUGMENT THIS DATA WITH AN ILR EXTRACT
                if (string.IsNullOrEmpty(tribalCourse.LearningAimRefId))
                {
                    providerReport += $"ATTENTION - Course does NOT have LARS and will NOT be migrated - ATTENTION" + Environment.NewLine;
                }
                else
                {
                    // IF QualificationType is missing, get it from LarsSearchService - ??? IF not found
                    if (string.IsNullOrEmpty(tribalCourse.Qualification))
                    {
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
                                providerReport += $"Course does NOT have QualificationType and we couldn't obtain it for LARS { tribalCourse.LearningAimRefId } " + Environment.NewLine;
                            }
                            else if (qualifications.Count.Equals(1))
                            {
                                tribalCourse.Qualification = qualifications[0];
                                providerReport += $"Your course did NOT have QualificationType, but we retrieve  the following '{ qualifications[0] }' for the LARS { tribalCourse.LearningAimRefId } " + Environment.NewLine;
                            }
                            else
                            {
                                string logMoreQualifications = string.Empty;
                                foreach (var qualification in qualifications)
                                {
                                    logMoreQualifications += "'" + qualification + "' ";
                                }

                                providerReport += $"Your course did NOT have QualificationType, but we retrieve { qualifications.Count.ToString() } qualifications for the LARS { tribalCourse.LearningAimRefId }, which are { logMoreQualifications } " + Environment.NewLine;
                            }
                        }
                        else
                        {
                            string logQualService = "Np Venue with this ????? - " + "" + ", Error: " + larsResult?.Error;
                        }
                    }

                    tribalCourse.AdvancedLearnerLoan = advancedLearnerLoan;

                    var tribalCourseRuns = DataHelper.GetCourseInstancesByCourseId(tribalCourse.CourseId, connectionString);

                    if (tribalCourseRuns != null)
                    {
                        tribalCourse.TribalCourseRuns = tribalCourseRuns;
                        foreach (var tribalCourseRun in tribalCourse.TribalCourseRuns)
                        {
                            tribalCourseRun.CourseName = tribalCourse.CourseTitle;
                            
                            // Call VenueService and for each tribalCourseRun.VenueId get tribalCourseRun.VenueGuidId (Only for type Location
                            if (tribalCourseRun.AttendanceType.Equals(AttendanceType.Location))
                            {
                                // DO NOT UNCOMMENT FOR TESTING ONLY
                                //tribalCourseRun.VenueId = 3497921;

                                if (tribalCourseRun.VenueId != null)
                                {
                                    GetVenueByVenueIdCriteria venueId = new GetVenueByVenueIdCriteria(tribalCourseRun.VenueId ?? 0);
                                    var venueResult = Task.Run(async () => await venueService.GetVenueByVenueIdAsync(venueId)).Result;

                                    if (venueResult.IsSuccess && venueResult.HasValue)
                                    {
                                        tribalCourseRun.VenueGuidId = new Guid(((Venue)venueResult.Value).ID);
                                        // Uncomment only in Development
                                        //providerReport += $"Venue Id '{ tribalCourseRun.VenueGuidId }' for GOOD " + Environment.NewLine;
                                    }
                                    else
                                    {
                                        tribalCourseRun.RecordStatus = RecordStatus.Pending;
                                        providerReport += $"ATTENTION - CourseRun - { tribalCourseRun.CourseInstanceId } - Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' - Venue with VenueId -  '{ tribalCourseRun.VenueId }' could not obtain VenueIdGuid , Error:  { venueResult?.Error } for BAD " + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    tribalCourseRun.RecordStatus = RecordStatus.Pending;
                                    providerReport += $"ATTENTION - NO Venue Id for CourseRun - { tribalCourseRun.CourseInstanceId } - Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' , although it's of  AttendanceType.Location" + Environment.NewLine;
                                }
                            }                         
                        }
                    }

                    // Do the mapping
                    var mappingMessages = new List<string>();
                    var course = MappingHelper.MapTribalCourseToCourse(tribalCourse, out mappingMessages);

                    foreach(var courseRun in course.CourseRuns)
                    {
                        providerReport += Environment.NewLine + $"- - - -  CourseRun { courseRun.CourseInstanceId } Ref: '{ courseRun.ProviderCourseID }' is migrated and has a RecordStatus: { courseRun.RecordStatus } " + Environment.NewLine;
                    }

                    foreach(var mappingMessage in mappingMessages)
                    {
                        providerReport += mappingMessage;
                    }

                    providerReport += Environment.NewLine + $"The Course has RecordStatus:  { course.RecordStatus } " + Environment.NewLine; ;

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
                        var courseResult = Task.Run(async () => await courseService.AddCourseAsync(course)).Result;

                        if (courseResult.IsSuccess && courseResult.HasValue)
                        {
                            providerReport += $"The course is migarted  " + Environment.NewLine;
                        }
                        else
                        {
                            // No
                            CountCourseFailures++;
                            providerReport += $"The course is NOT migrated.  " + Environment.NewLine;
                        }
                    }
                }
            }

            providerReport += $"Courses { CountCourseFailures } Failures  " + Environment.NewLine;
            string providerReportFileName = string.Format("{0}-MigrationReport-{1}-{2}.txt", DateTime.Now.ToString("yyMMdd-HHmmss"), UKPRN, tribalCourses.Count.ToString());
            File.WriteAllText(string.Format(@"{0}\ProviderReports\{1}", jsonCourseFilesPath, providerReportFileName), providerReport);

            providerStopWatch.Stop();
            adminReport += $"Report { reportForProvider } - { providerReportFileName } - Time taken: { providerStopWatch.Elapsed } " + Environment.NewLine;
            adminReport += $"Number of Courses { tribalCourses.Count } ? Pending ? Live ?" + Environment.NewLine;

            adminStopWatch.Stop();
            //string formatedStopWatchElapsedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}:{4:D3}", stopWatch.Elapsed.Days, stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds);
            adminReport += Environment.NewLine + $"Total time taken: { adminStopWatch.Elapsed } " + Environment.NewLine;
            string adminReportFileName = string.Format("{0}-AdminReport-{1}.txt", DateTime.Now.ToString("yyMMdd-HHmmss"), CountProviders.ToString());
            File.WriteAllText(string.Format(@"{0}\AdminReports\{1}", jsonCourseFilesPath, adminReportFileName), adminReport);

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
