using Dfc.CourseDirectory.Common.Settings;
using Dfc.CourseDirectory.CourseMigrationTool.Helpers;
using Dfc.CourseDirectory.CourseMigrationTool.Models;
using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Helpers;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Models.Models.Providers;
using Dfc.CourseDirectory.Models.Models.Venues;
using Dfc.CourseDirectory.Services;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.CourseTextService;
using Dfc.CourseDirectory.Services.Interfaces;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.CourseTextService;
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
                .Configure<CourseForComponentSettings>(CourseForComponentSettingsOptions =>
                { CourseForComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:CourseForComponentSettings:TextFieldMaxChars"); })
                .Configure<EntryRequirementsComponentSettings>(EntryRequirementsComponentSettingsOptions =>
                { EntryRequirementsComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:EntryRequirementsComponentSettings:TextFieldMaxChars"); })
                .Configure<WhatWillLearnComponentSettings>(WhatWillLearnComponentSettingsOptions =>
                { WhatWillLearnComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:WhatWillLearnComponentSettings:TextFieldMaxChars"); })
                .Configure<HowYouWillLearnComponentSettings>(HowYouWillLearnComponentSettingsOptions =>
                { HowYouWillLearnComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:HowYouWillLearnComponentSettings:TextFieldMaxChars"); })
                .Configure<WhatYouNeedComponentSettings>(WhatYouNeedComponentSettingsOptions =>
                { WhatYouNeedComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:WhatYouNeedComponentSettings:TextFieldMaxChars"); })
                .Configure<HowAssessedComponentSettings>(HowAssessedComponentSettingsOptions =>
                { HowAssessedComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:HowAssessedComponentSettings:TextFieldMaxChars"); })
                .Configure<WhereNextComponentSettings>(WhereNextComponentSettingsOptions =>
                { WhereNextComponentSettingsOptions.TextFieldMaxChars = configuration.GetValue<int>("AppUISettings:WhereNextComponentSettings:TextFieldMaxChars"); })
                .Configure<CourseServiceSettings>(courseServiceSettingsOptions =>
                {
                    courseServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("CourseServiceSettings:ApiUrl");
                    courseServiceSettingsOptions.ApiKey = configuration.GetValue<string>("CourseServiceSettings:ApiKey");
                }
                )
                .AddScoped<ICourseService, CourseService>()
                .Configure<CourseTextServiceSettings>(courseTextServiceSettingsOptions =>
                {
                    courseTextServiceSettingsOptions.ApiUrl = configuration.GetValue<string>("CourseTextServiceSettings:ApiUrl");
                    courseTextServiceSettingsOptions.ApiKey = configuration.GetValue<string>("CourseTextServiceSettings:ApiKey");
                }
                )
                .AddScoped<ICourseTextService, CourseTextService>()
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
            var courseTextService = serviceProvider.GetService<ICourseTextService>();
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
            int numberOfMonthsAgo = configuration.GetValue<int>("NumberOfMonthsAgo");
            bool dummyMode = configuration.GetValue<bool>("DummyMode");
            bool DeleteCoursesByUKPRN = configuration.GetValue<bool>("DeleteCoursesByUKPRN");
            bool EnableProviderOnboarding = configuration.GetValue<bool>("EnableProviderOnboarding");

            #endregion 

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
                Console.WriteLine("The Migration Tool is running in Automated Mode." + Environment.NewLine + "Please, do not close this window until \"Migration completed\" message is displayed." + Environment.NewLine);

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
                Console.WriteLine("Please enter valid UKPRN to migrate courses for a single Provider" + Environment.NewLine + "or \"s\" to migrate courses for a selection of Providers:" + 
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

            if (goodToTransfer)
            {
                string errorMessageCourseTransferAdd = string.Empty;
                if (providerUKPRNList != null && providerUKPRNList.Count > 0)
                {
                    DataHelper.CourseTransferAdd(connectionString,
                                                    DateTime.Now,
                                                    (int)transferMethod,
                                                    (int)deploymentEnvironment,
                                                    string.Empty,
                                                    "DFC – Course Migration Tool",
                                                    singleProviderUKPRN,
                                                    out errorMessageCourseTransferAdd,
                                                    out courseTransferId);
                }
                if (!string.IsNullOrEmpty(errorMessageCourseTransferAdd)) adminReport += errorMessageCourseTransferAdd + Environment.NewLine;

                if (courseTransferId.Equals(-1))
                {
                    adminReport += $"We cannot get the BatchNumber (CourseTransferId), so migration will be terminated. Number of UKPRNs ( { providerUKPRNList?.Count } )" + Environment.NewLine;
                    providerUKPRNList = null;
                }
            }


            Stopwatch adminStopWatch = new Stopwatch();
            adminStopWatch.Start();
            Stopwatch provStopWatch = new Stopwatch();
            provStopWatch.Start();

            int CountProviders = 0;
            //int CountProvidersGoodToMigrate = 0;
            int CountProvidersNotGoodToMigrate = 0;


            int CountAllCourses = 0;
            //int CountAllCourseRunsToBeMigrated = 0;
            int CountAllCoursesGoodToMigrate = 0;
            int CountAllCoursesNotGoodToMigrate = 0;
            int CountAllCoursesPending = 0;
            int CountAllCoursesLive = 0;
            int CountAllCoursesLARSless = 0;
            int CountCourseMigrationSuccess = 0;
            int CountCourseMigrationFailure = 0;
            //int CountAllCoursesMigrated = 0;
            //int CountAllCoursesNotMigrated = 0;
            int CountAllCourseRuns = 0;
            int CountAllCourseRunsLive = 0;
            int CountAllCourseRunsPending = 0;
            int CountAllCourseRunsReadyToGoLive = 0;
            int CountAllCourseRunsLARSless = 0;
            int InvalidCharCount = 0;

            #endregion

            foreach (var providerUKPRN in providerUKPRNList)
            {
                CountProviders++;
                Console.WriteLine("Doing: " + providerUKPRN.ToString());
                Console.WriteLine("Count: " + CountProviders.ToString());
               

                int CountCourseRunsToBeMigrated = 0;
                int CountCourseInvalid = 0;
                int CountCourseValid = 0;
                int CountCourseGoodToMigrate = 0;
                int CountCourseNotGoodToMigrate = 0;
                int CountCourseLARSless = 0;
                int CountProviderCourseMigrationSuccess = 0;
                int CountProviderCourseMigrationFailure = 0;

                int CountProviderCourseRuns = 0;
                int CountProviderCourseRunsLive = 0;
                int CountProviderCourseRunsPending = 0;
                int CountProviderCourseRunsReadyToGoLive = 0;
                int CountProviderCourseRunsLARSless = 0;
                //InvalidCharCount = 0;


                string providerReport = "                         Migration Report " + Environment.NewLine;

                Stopwatch providerStopWatch = new Stopwatch();
                providerStopWatch.Start();

                string providerName = string.Empty;
                bool advancedLearnerLoan = false;
                string errorMessageGetCourses = string.Empty;
                var tribalCourses = DataHelper.GetCoursesByProviderUKPRN(providerUKPRN, connectionString, out providerName, out advancedLearnerLoan, out errorMessageGetCourses);
                if (!string.IsNullOrEmpty(errorMessageGetCourses)) adminReport += errorMessageGetCourses + Environment.NewLine;

                string reportForProvider = $"for Provider '{ providerName }' with UKPRN  ( { providerUKPRN } )";
                providerReport += reportForProvider + Environment.NewLine + Environment.NewLine;
                providerReport += "________________________________________________________________________________" + Environment.NewLine + Environment.NewLine;


                if (EnableProviderOnboarding)
                {
                    // Check whether Provider is Onboarded
                    var providerCriteria = new ProviderSearchCriteria(providerUKPRN.ToString());
                    var providerResult = Task.Run(async () => await providerService.GetProviderByPRNAsync(providerCriteria)).Result;

                    if (providerResult.IsSuccess && providerResult.HasValue)
                    {
                        var providers = providerResult.Value.Value;
                        if (providers.Count().Equals(1))
                        {
                            var provider = providers.FirstOrDefault();
                            if (provider.Status.Equals(Status.Onboarded))
                            {
                                providerReport += $"Provider WAS already ONBOARDED" + Environment.NewLine + Environment.NewLine;
                            }
                            else
                            {
                                if (provider.ProviderStatus.Equals("Active", StringComparison.InvariantCultureIgnoreCase)
                                    || provider.ProviderStatus.Equals("Verified", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    // Onboard the Provider
                                    ProviderAdd providerOnboard = new ProviderAdd(provider.id, (int)Status.Onboarded, "DFC – Course Migration Tool");
                                    var resultProviderOnboard = Task.Run(async () => await providerService.AddProviderAsync(providerOnboard)).Result;
                                    if (resultProviderOnboard.IsSuccess && resultProviderOnboard.HasValue)
                                    {
                                        providerReport += $"We HAVE ONBOARDED the Provider" + Environment.NewLine + Environment.NewLine;
                                    }
                                    else
                                    {
                                        providerReport += $"ERROR on ONBOARDING the Provider - { resultProviderOnboard.Error }" + Environment.NewLine + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    providerReport += $"Provider CANNOT be ONBOARDED" + Environment.NewLine + Environment.NewLine;
                                }
                            }
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
                }


                if (DeleteCoursesByUKPRN)
                {
                    providerReport += $"ATTENTION - Existing Courses for Provider '{ providerName }' with UKPRN  ( { providerUKPRN } ) to be deleted." + Environment.NewLine;

                    // Call the service 
                    var deleteCoursesByUKPRNResult = Task.Run(async () => await courseService.DeleteCoursesByUKPRNAsync(new DeleteCoursesByUKPRNCriteria(providerUKPRN))).Result;

                    if (deleteCoursesByUKPRNResult.IsSuccess && deleteCoursesByUKPRNResult.HasValue)
                    {
                        providerReport += $"The deleted courses:  " + Environment.NewLine;
                        // StatusCode => NoContent = 204 is good 
                        foreach (var deleteMessage in deleteCoursesByUKPRNResult.Value)
                        {
                            providerReport += deleteMessage + Environment.NewLine;
                        }
                    }
                    else
                    {
                        providerReport += $"Error on delteing courses -  { deleteCoursesByUKPRNResult.Error }  " + Environment.NewLine;
                    }
                }

                foreach (var tribalCourse in tribalCourses)
                {
                    string preserveCourseTitle = tribalCourse.CourseTitle;
                    string courseReport = Environment.NewLine + $"Course Report" + Environment.NewLine;
                    courseReport += "________________________________________________________________________________" + Environment.NewLine;

                    var course = new Course();
                    var migrationSuccess = new MigrationSuccess();
                    migrationSuccess = MigrationSuccess.Undefined;

                    courseReport += Environment.NewLine + $">>> Course { tribalCourse.CourseId } LARS: { tribalCourse.LearningAimRefId } and Title ( { tribalCourse.CourseTitle } ) to be migrated " + Environment.NewLine;

                    bool LARSlessCourse = false;
                    // DO NOT MIGRATE COURSES WITHOUT A LARS REFERENCE. WE WILL LOOK TO AUGMENT THIS DATA WITH AN ILR EXTRACT
                    if (string.IsNullOrEmpty(tribalCourse.LearningAimRefId))
                    {
                        LARSlessCourse = true;
                        courseReport += $"ATTENTION - LARSless Course - Course does NOT have LARS - ATTENTION" + Environment.NewLine;
                        //CountCourseNotGoodToMigrate++;
                    }
                    else
                    {
                        //tribalCourse.LearningAimRefId = "6538787SD"; // For Testing Only
                        // Replace LearnAimRefTitle, NotionalNVQLevelv2, AwardOrgCode, QualificationType from LarsSearchService 
                        LarsSearchCriteria criteria = new LarsSearchCriteria(tribalCourse.LearningAimRefId, 10, 0, string.Empty, null);
                        var larsResult = Task.Run(async () => await larsSearchService.SearchAsync(criteria)).Result;

                        if (larsResult.IsSuccess && larsResult.HasValue)
                        {
                            var qualifications = new List<LarsDataResultItem>();
                            foreach (var item in larsResult.Value.Value)
                            {
                                var larsDataResultItem = new LarsDataResultItem
                                {
                                    LearnAimRefTitle = item.LearnAimRefTitle,
                                    NotionalNVQLevelv2 = item.NotionalNVQLevelv2,
                                    AwardOrgCode = item.AwardOrgCode,
                                    LearnAimRefTypeDesc = item.LearnAimRefTypeDesc,
                                    CertificationEndDate = item.CertificationEndDate
                                };
                                qualifications.Add(larsDataResultItem);
                            }

                            if (qualifications.Count.Equals(0))
                            {
                                LARSlessCourse = true;
                                //CountCourseNotGoodToMigrate++;
                                courseReport += $"ATTENTION - LARSless Course - We couldn't obtain LARS Data for LARS: { tribalCourse.LearningAimRefId }. LARS Service returns nothing." + Environment.NewLine;
                            }
                            else if (qualifications.Count.Equals(1))
                            {
                                if (qualifications[0].CertificationEndDate != null && qualifications[0].CertificationEndDate < DateTime.Now)
                                {
                                    // Expired LARS
                                    LARSlessCourse = true;
                                    //CountCourseNotGoodToMigrate++;
                                    courseReport += $"ATTENTION - LARSless Course - LARS has expired for LARS: { tribalCourse.LearningAimRefId }. The CertificationEndDate is { qualifications[0].CertificationEndDate }" + Environment.NewLine;
                                }
                                else
                                {
                                    // We continue only if we could much LARS
                                    tribalCourse.CourseTitle = qualifications[0].LearnAimRefTitle;
                                    tribalCourse.QualificationLevelIdString = qualifications[0].NotionalNVQLevelv2;
                                    tribalCourse.LearningAimAwardOrgCode = qualifications[0].AwardOrgCode;
                                    tribalCourse.Qualification = qualifications[0].LearnAimRefTypeDesc;
                                }
                            }
                            else
                            {
                                LARSlessCourse = true;
                                //CountCourseNotGoodToMigrate++;
                                string logMoreQualifications = string.Empty;
                                foreach (var qualification in qualifications)
                                {
                                    logMoreQualifications += "( '" + qualification.LearnAimRefTitle + "' with Level " + qualification.NotionalNVQLevelv2 + " and AwardOrgCode " + qualification.AwardOrgCode + " ) ";
                                }
                                courseReport += $"ATTENTION - LARSless Course - We retrieve multiple qualifications ( { qualifications.Count.ToString() } ) for the LARS { tribalCourse.LearningAimRefId }, which are { logMoreQualifications } " + Environment.NewLine;
                            }
                        }
                        else
                        {
                            LARSlessCourse = true;
                            //CountCourseNotGoodToMigrate++;
                            courseReport += $"ATTENTION - LARSless Course - We couldn't retreive LARS data for LARS { tribalCourse.LearningAimRefId }, because of technical reason, Error: " + larsResult?.Error;
                        }
                    }

                    // Do not migrate courses with BAD LARS
                    //if (!LARSlessCourse)
                    //{
                    // If there is no CourseFor Text we getting it from CourseTextService
                    if (string.IsNullOrEmpty(tribalCourse.CourseSummary))
                    {
                        courseReport += $"The course with LARS { tribalCourse.LearningAimRefId } did not have CourseSummary, which is required." + Environment.NewLine;
                        var courseTextResult = courseTextService.GetCourseTextByLARS(new CourseTextServiceCriteria(tribalCourse.LearningAimRefId)).Result;

                        if (courseTextResult.IsSuccess && courseTextResult.HasValue)
                        {
                            tribalCourse.CourseSummary = courseTextResult.Value?.CourseDescription;

                            courseReport += $"And we have placed exemplar content for it." + Environment.NewLine;
                        }
                        else
                        {
                            courseReport += $"And we have tried to place exemplar content for it. Unfortunately, we couldn’t do it. Error -  { courseTextResult.Error }  " + Environment.NewLine;
                        }
                    }

                    tribalCourse.AdvancedLearnerLoan = advancedLearnerLoan;

                    string errorMessageGetCourseRuns = string.Empty;
                    var tribalCourseRuns = DataHelper.GetCourseInstancesByCourseId(tribalCourse.CourseId, connectionString, out errorMessageGetCourseRuns);
                    if (!string.IsNullOrEmpty(errorMessageGetCourseRuns)) adminReport += errorMessageGetCourseRuns + Environment.NewLine;

                    if (tribalCourseRuns != null && tribalCourseRuns.Count > 0)
                    {
                        CountCourseRunsToBeMigrated = CountCourseRunsToBeMigrated + tribalCourseRuns.Count;
                        tribalCourse.TribalCourseRuns = tribalCourseRuns;
                        foreach (var tribalCourseRun in tribalCourse.TribalCourseRuns)
                        {
                            tribalCourseRun.CourseName = preserveCourseTitle; //tribalCourse.CourseTitle;

                            // Call VenueService and for each tribalCourseRun.VenueId get tribalCourseRun.VenueGuidId (Applicable only for type Location/Classroom)
                            if (tribalCourseRun.AttendanceType.Equals(AttendanceType.Location))
                            {
                                if (tribalCourseRun.VenueId != null)
                                {
                                    GetVenueByVenueIdCriteria venueId = new GetVenueByVenueIdCriteria(tribalCourseRun.VenueId ?? 0);
                                    var venueResult = Task.Run(async () => await venueService.GetVenueByVenueIdAsync(venueId)).Result;

                                    if (venueResult.IsSuccess && venueResult.HasValue)
                                    {
                                        tribalCourseRun.VenueGuidId = new Guid(((Venue)venueResult.Value).ID);
                                    }
                                    else
                                    {
                                        tribalCourseRun.RecordStatus = RecordStatus.MigrationPending;
                                        courseReport += $"ATTENTION - CourseRun - { tribalCourseRun.CourseInstanceId } - Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' - Venue with VenueId -  '{ tribalCourseRun.VenueId }' could not obtain VenueIdGuid , Error:  { venueResult?.Error } for BAD " + Environment.NewLine;
                                    }
                                }
                                else
                                {
                                    tribalCourseRun.RecordStatus = RecordStatus.MigrationPending;
                                    courseReport += $"ATTENTION - NO Venue Id for CourseRun - { tribalCourseRun.CourseInstanceId } - Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' , although it's of  AttendanceType.Location" + Environment.NewLine;
                                }
                            }
                        }

                        // Process the Course and CourseRuns.
                        // Do the mapping
                        var mappingMessages = new List<string>();
                        bool courseNOTtoBeMigrated = false;
                        course = MappingHelper.MapTribalCourseToCourse(tribalCourse, numberOfMonthsAgo, dummyMode, out mappingMessages, out courseNOTtoBeMigrated);


                        if (courseNOTtoBeMigrated)
                        {
                            courseReport += $"ATTENTION - The Course does not have any CourseRuns and will NOT be migrated - ATTENTION" + Environment.NewLine;
                            CountCourseNotGoodToMigrate++;
                        }
                        else
                        {
                            // Validate Course
                            var courseValidationMessages = courseService.ValidateCourse(course);
                            course.IsValid = courseValidationMessages.Any() ? false : true;
                            courseReport += Environment.NewLine + $"Course Validation Messages:  " + Environment.NewLine; ;
                            foreach (var courseValidationMessage in courseValidationMessages)
                            {
                                courseReport += courseValidationMessage + Environment.NewLine;
                                if (courseValidationMessage.Contains("invalid character")) InvalidCharCount++;
                            }
                            courseReport += Environment.NewLine + $"The Course IsValid property:  { course.IsValid } " + Environment.NewLine; ;

                            //if (BitmaskHelper.IsSet(course.CourseStatus, RecordStatus.Live)) CountCourseValid++; // ???
                            //if (BitmaskHelper.IsSet(course.CourseStatus, RecordStatus.Pending)) CountCoursePending++;

                            if (course.IsValid.Equals(true) && !LARSlessCourse) CountCourseValid++;
                            if (course.IsValid.Equals(false) && !LARSlessCourse) CountCourseInvalid++;

                            // Validate CourseRuns
                            courseReport += Environment.NewLine + $"CourseRuns Validation Messages: " + Environment.NewLine; ;
                            foreach (var courseRun in course.CourseRuns)
                            {
                                var courseRunValidationMessages = courseService.ValidateCourseRun(courseRun, ValidationMode.MigrateCourse);
                                courseRun.RecordStatus = courseRunValidationMessages.Any() ? RecordStatus.MigrationPending : RecordStatus.Live;
                                if (course.IsValid.Equals(false))
                                {
                                    if (courseRun.RecordStatus.Equals(RecordStatus.Live))
                                    {
                                        courseRun.RecordStatus = RecordStatus.MigrationReadyToGoLive;
                                    }
                                }
                                courseReport += Environment.NewLine + $"- - - CourseRun { courseRun.CourseInstanceId } Ref: '{ courseRun.ProviderCourseID }' CourseName: {courseRun.CourseName }" + Environment.NewLine +
                                    $"DeliveryMode: { courseRun.DeliveryMode},  StudyMode: { courseRun.StudyMode } is migrated and has a RecordStatus: { courseRun.RecordStatus } " + Environment.NewLine;

                                foreach (var courseRunValidationMessage in courseRunValidationMessages)
                                {
                                    courseReport += courseRunValidationMessage + Environment.NewLine; ;
                                }
                            }

                            if (mappingMessages != null && mappingMessages.Count > 0)
                            {
                                courseReport += Environment.NewLine + $"CourseRuns Mapping Messages: " + Environment.NewLine;
                                foreach (var mappingMessage in mappingMessages)
                                {
                                    courseReport += mappingMessage + Environment.NewLine;
                                }
                            }

                            if (LARSlessCourse)
                            {
                                foreach (var courseRun in course.CourseRuns)
                                {
                                    courseRun.RecordStatus = RecordStatus.LARSless;
                                }
                            }

                            // Migrate Course 
                            if (generateJsonFilesLocally)
                            {
                                if (LARSlessCourse)
                                {
                                    // Output LARSless Courses JSON to different folder
                                    var courseJson = JsonConvert.SerializeObject(course);
                                    string jsonFileName = string.Format("{0}-{1}-{2}-{3}-{4}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), course.ProviderUKPRN, course.LearnAimRef, course.CourseId, GetCourseRunsCount(course.CourseRuns).ToString());
                                    string LARSlessCoursesPath = string.Format(@"{0}\LARSlessCourses", jsonCourseFilesPath);
                                    if (!Directory.Exists(LARSlessCoursesPath))
                                        Directory.CreateDirectory(LARSlessCoursesPath);
                                    File.WriteAllText(string.Format(@"{0}\{1}", LARSlessCoursesPath, jsonFileName), courseJson);
                                }
                                else
                                {
                                    var courseJson = JsonConvert.SerializeObject(course);
                                    string jsonFileName = string.Format("{0}-{1}-{2}-{3}-{4}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), course.ProviderUKPRN, course.LearnAimRef, course.CourseId, GetCourseRunsCount(course.CourseRuns).ToString());
                                    if (!Directory.Exists(jsonCourseFilesPath))
                                        Directory.CreateDirectory(jsonCourseFilesPath);
                                    File.WriteAllText(string.Format(@"{0}\{1}", jsonCourseFilesPath, jsonFileName), courseJson);
                                }
                            }
                            else
                            {
                                if (LARSlessCourse)
                                {
                                    // Output LARSless Courses JSON to different folder
                                    var courseJson = JsonConvert.SerializeObject(course);
                                    string jsonFileName = string.Format("{0}-{1}-{2}-{3}-{4}.json", DateTime.Now.ToString("yyMMdd-HHmmss"), course.ProviderUKPRN, course.LearnAimRef, course.CourseId, GetCourseRunsCount(course.CourseRuns).ToString());
                                    string LARSlessCoursesPath = string.Format(@"{0}\LARSlessCourses", jsonCourseFilesPath);
                                    if (!Directory.Exists(LARSlessCoursesPath))
                                        Directory.CreateDirectory(LARSlessCoursesPath);
                                    File.WriteAllText(string.Format(@"{0}\{1}", LARSlessCoursesPath, jsonFileName), courseJson);
                                }
                                else
                                {
                                    // Call the service
                                    var courseResult = Task.Run(async () => await courseService.AddCourseAsync(course)).Result;

                                    if (courseResult.IsSuccess && courseResult.HasValue)
                                    {
                                        CountProviderCourseMigrationSuccess++;
                                        courseReport += Environment.NewLine + $"The course is migrated  " + Environment.NewLine;
                                        migrationSuccess = MigrationSuccess.Success;
                                    }
                                    else
                                    {
                                        CountProviderCourseMigrationFailure++;
                                        courseReport += Environment.NewLine + $"The course is NOT migrated. Error -  { courseResult.Error }  " + Environment.NewLine;
                                        migrationSuccess = MigrationSuccess.Failure;
                                    }
                                }
                            }
                        }
                        //} // END BAD LARS IF
                    }
                    else
                    {
                        // A decision was made NOT to migrate courses that have no Course Runs associated to that course
                        courseReport += $"ATTENTION - Course does NOT have CourseRuns associated with it and will NOT be migrated - ATTENTION" + Environment.NewLine;
                        CountCourseNotGoodToMigrate++;
                    }


                    // Course Auditing 
                    int courseRunsLive = 0;
                    int courseRunsPending = 0;
                    int courseRunsReadyToGoLive = 0;
                    int courseRunsLARSless = 0;
                    foreach (var courseRun in course.CourseRuns ?? Enumerable.Empty<CourseRun>())
                    {
                        if (courseRun.RecordStatus.Equals(RecordStatus.Live)) courseRunsLive++;
                        if (courseRun.RecordStatus.Equals(RecordStatus.MigrationPending)) courseRunsPending++;
                        if (courseRun.RecordStatus.Equals(RecordStatus.MigrationReadyToGoLive)) courseRunsReadyToGoLive++;
                        if (courseRun.RecordStatus.Equals(RecordStatus.LARSless)) courseRunsLARSless++;
                    }

                    CountProviderCourseRuns = CountProviderCourseRuns + GetCourseRunsCount(course?.CourseRuns);
                    CountProviderCourseRunsLive = CountProviderCourseRunsLive + courseRunsLive;
                    CountProviderCourseRunsPending = CountProviderCourseRunsPending + courseRunsPending;
                    CountProviderCourseRunsReadyToGoLive = CountProviderCourseRunsReadyToGoLive + courseRunsReadyToGoLive;
                    CountProviderCourseRunsLARSless = CountProviderCourseRunsLARSless + courseRunsLARSless;

                    string errorMessageCourseAuditAdd = string.Empty;
                    DataHelper.CourseTransferCourseAuditAdd(connectionString,
                                               courseTransferId,
                                               providerUKPRN,
                                               course?.CourseId ?? 0,
                                               course?.LearnAimRef,
                                               (int)course?.CourseStatus,
                                               GetCourseRunsCount(course?.CourseRuns),
                                               courseRunsLive,
                                               courseRunsPending,
                                               courseRunsReadyToGoLive,
                                               courseRunsLARSless,
                                               (int)migrationSuccess,
                                               courseReport,
                                               out errorMessageCourseAuditAdd);
                    if (!string.IsNullOrEmpty(errorMessageCourseAuditAdd)) adminReport += "Error on CourseTransferCourseAuditAdd:" + errorMessageCourseAuditAdd + Environment.NewLine;

                    // Attach courseReport to providerReport
                    courseReport += "________________________________________________________________________________" + Environment.NewLine;
                    providerReport += courseReport;

                    if (LARSlessCourse) CountCourseLARSless++;
                    else CountCourseGoodToMigrate++;
                }

                //CountCourseGoodToMigrate = tribalCourses.Count - CountCourseNotGoodToMigrate;
                
                providerReport += "________________________________________________________________________________" + Environment.NewLine + Environment.NewLine;

                string coursesToBeMigrated = $"( { tribalCourses.Count } ) Courses with ( { CountCourseRunsToBeMigrated } ) CourseRuns to be migrated ";
                if (tribalCourses.Count.Equals(0))
                {
                    CountProvidersNotGoodToMigrate++;
                    coursesToBeMigrated = $"Number of Courses to be migrated is ( { tribalCourses.Count }), which means that either the Provider is NOT Live or is Live, but does not have any live courses. In either case we don't have courses to be migrated";
                }

                providerReport += coursesToBeMigrated + Environment.NewLine;
                providerReport += $"Number of good to migrate Courses ( { CountCourseGoodToMigrate } ) - Invalid ( { CountCourseInvalid} ) and Valid ( { CountCourseValid } )" + Environment.NewLine;
                providerReport += $"Number of LARSless Courses ( { CountCourseLARSless } )" + Environment.NewLine;
                providerReport += $"Number of NOT good to migrate Courses ( { CountCourseNotGoodToMigrate } )" + Environment.NewLine;
                providerReport += $"Number of good to migrate CourseRuns ( { CountProviderCourseRuns } ) - Live ( { CountProviderCourseRunsLive } ), MigrationPending ( { CountProviderCourseRunsPending } ),  MigrationReadyToGoLive ( { CountProviderCourseRunsReadyToGoLive } ) and LARSless ( { CountProviderCourseRunsLARSless } )" + Environment.NewLine;
                providerReport += $"Courses Migration Successes ( { CountProviderCourseMigrationSuccess } ) and Failures ( { CountProviderCourseMigrationFailure } )" + Environment.NewLine;

                string providerReportFileName = string.Empty;
                if (generateReportFilesLocally)
                {
                    providerReportFileName = string.Format("{0}-MigrationReport-{1}-{2}.txt", DateTime.Now.ToString("yyMMdd-HHmmss"), providerUKPRN, tribalCourses.Count.ToString());
                    string ProviderReportsPath = string.Format(@"{0}\ProviderReports", jsonCourseFilesPath);
                    if (!Directory.Exists(ProviderReportsPath))
                        Directory.CreateDirectory(ProviderReportsPath);
                    File.WriteAllText(string.Format(@"{0}\{1}", ProviderReportsPath, providerReportFileName), providerReport);
                }

                providerStopWatch.Stop();
                adminReport += $">>> Report { reportForProvider } - { providerReportFileName } - Time taken: { providerStopWatch.Elapsed } " + Environment.NewLine;
                adminReport += coursesToBeMigrated + Environment.NewLine;
                adminReport += $"Number of good to migrate Courses ( { CountCourseGoodToMigrate } ) - Invalid ( { CountCourseInvalid} ) and Valid ( { CountCourseValid } )" + Environment.NewLine;
                adminReport += $"Number of LARSless Courses ( { CountCourseLARSless } )" + Environment.NewLine;
                adminReport += $"Number of NOT good to migrate Courses ( { CountCourseNotGoodToMigrate } )" + Environment.NewLine;
                adminReport += $"Number of good to migrate CourseRuns ( { CountProviderCourseRuns } ) - Live ( { CountProviderCourseRunsLive } ), MigrationPending ( { CountProviderCourseRunsPending } ),  MigrationReadyToGoLive ( { CountProviderCourseRunsReadyToGoLive } ) and LARSless ( { CountProviderCourseRunsLARSless } )" + Environment.NewLine;
                adminReport += $"Courses Migration Successes ( { CountProviderCourseMigrationSuccess } ) and Failures ( { CountProviderCourseMigrationFailure } )" + Environment.NewLine + Environment.NewLine;

                // Provider Auditing 
                string errorMessageProviderAuditAdd = string.Empty;
                DataHelper.CourseTransferProviderAuditAdd(connectionString,
                                                           courseTransferId,
                                                           providerUKPRN,
                                                           tribalCourses.Count,
                                                           CountCourseGoodToMigrate,
                                                           CountCourseInvalid,
                                                           CountCourseValid,
                                                           CountCourseNotGoodToMigrate,
                                                           CountCourseLARSless,
                                                           CountProviderCourseMigrationSuccess,
                                                           CountProviderCourseMigrationFailure,
                                                           providerReportFileName,
                                                           providerStopWatch.Elapsed.ToString(),
                                                           providerReport,
                                                           out errorMessageProviderAuditAdd);
                if (!string.IsNullOrEmpty(errorMessageProviderAuditAdd)) adminReport += "Error on CourseTransferProviderAuditAdd:" + errorMessageProviderAuditAdd + Environment.NewLine;


                CountAllCourses = CountAllCourses + tribalCourses.Count;
                CountAllCoursesGoodToMigrate = CountAllCoursesGoodToMigrate + CountCourseGoodToMigrate;
                CountAllCoursesNotGoodToMigrate = CountAllCoursesNotGoodToMigrate + (tribalCourses.Count - CountCourseGoodToMigrate);
                CountAllCoursesPending = CountAllCoursesPending + CountCourseInvalid;
                CountAllCoursesLive = CountAllCoursesLive + CountCourseValid;
                CountAllCoursesLARSless = CountAllCoursesLARSless + CountCourseLARSless;
                CountCourseMigrationSuccess = CountCourseMigrationSuccess + CountProviderCourseMigrationSuccess;
                CountCourseMigrationFailure = CountCourseMigrationFailure + CountProviderCourseMigrationFailure;

                CountAllCourseRuns = CountAllCourseRuns + CountProviderCourseRuns;
                CountAllCourseRunsLive = CountAllCourseRunsLive + CountProviderCourseRunsLive;
                CountAllCourseRunsPending = CountAllCourseRunsPending + CountProviderCourseRunsPending;
                CountAllCourseRunsReadyToGoLive = CountAllCourseRunsReadyToGoLive + CountProviderCourseRunsReadyToGoLive;
                CountAllCourseRunsLARSless = CountAllCourseRunsLARSless + CountProviderCourseRunsLARSless;

                // For feedback to the user only
                provStopWatch.Stop();
                //string formatedStopWatchElapsedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}:{4:D3}", stopWatch.Elapsed.Days, stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds);
                Console.WriteLine("Total time taken:" + provStopWatch.Elapsed.ToString());
                provStopWatch.Start();
            }
           Console.WriteLine("Invalid Char count: " + InvalidCharCount.ToString());
            // Finish Admin Report
            adminReport += "________________________________________________________________________________" + Environment.NewLine + Environment.NewLine;
            adminStopWatch.Stop();
            //string formatedStopWatchElapsedTime = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}:{4:D3}", stopWatch.Elapsed.Days, stopWatch.Elapsed.Hours, stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds, stopWatch.Elapsed.Milliseconds);
            adminReport += $"Number of Providers to be migrated ( { CountProviders } ). Total time taken: { adminStopWatch.Elapsed } " + Environment.NewLine;
            adminReport += $"Number of migrated Providers ( { CountProviders - CountProvidersNotGoodToMigrate } ). Number of Providers which are not active or not have any courses ( { CountProvidersNotGoodToMigrate } )" + Environment.NewLine;
            adminReport += $"Total number of Courses processed ( { CountAllCourses } )." + Environment.NewLine;
            adminReport += $"Total number of GOOD to migrate Courses ( { CountAllCoursesGoodToMigrate} ) and total number of NOT good to migrate courses ( { CountAllCoursesNotGoodToMigrate } )" + Environment.NewLine;
            adminReport += $"Total number of GOOD to migrate Courses with Invalid status  ( { CountAllCoursesPending} ) and Valid status ( { CountAllCoursesLive } )" + Environment.NewLine;
            adminReport += $"Total number of LARSless Courses processed ( { CountAllCoursesLARSless } )." + Environment.NewLine;
            adminReport += $"Total number of GOOD to migrate CourseRuns ( { CountAllCourseRuns } ) with Live status ( { CountAllCourseRunsLive } ), MigrationPending status  ( { CountAllCourseRunsPending } ), MigrationReadyToGoLive status  ( { CountAllCourseRunsReadyToGoLive } ) and LARSless status ( { CountAllCourseRunsLARSless } )" + Environment.NewLine;
            adminReport += $"Total number of courses migrated ( { CountCourseMigrationSuccess } ) and total number of NOT migrated courses ( { CountCourseMigrationFailure } )" + Environment.NewLine;

            string adminReportFileName = string.Empty;
            if (generateReportFilesLocally)
            {
                adminReportFileName = string.Format("{0}-AdminReport-{1}.txt", DateTime.Now.ToString("yyMMdd-HHmmss"), CountProviders.ToString());
                string AdminReportsPath = string.Format(@"{0}\AdminReports", jsonCourseFilesPath);
                if (!Directory.Exists(AdminReportsPath))
                    Directory.CreateDirectory(AdminReportsPath);
                File.WriteAllText(string.Format(@"{0}\{1}", AdminReportsPath, adminReportFileName), adminReport);
            }

            // Transfer Auditing 
            string errorMessageCourseTransferUpdate = string.Empty;
            DataHelper.CourseTransferUpdate(connectionString,
                                                courseTransferId,
                                                CountProviders,
                                                CountProviders - CountProvidersNotGoodToMigrate,
                                                CountProvidersNotGoodToMigrate,
                                                CountAllCourses,
                                                CountAllCoursesGoodToMigrate,
                                                CountAllCoursesNotGoodToMigrate,
                                                CountAllCoursesLive,
                                                CountAllCoursesPending,
                                                CountAllCoursesLARSless,
                                                CountCourseMigrationSuccess,
                                                CountCourseMigrationFailure,
                                                DateTime.Now,
                                                adminStopWatch.Elapsed.ToString(),
                                                bulkUploadFileName,
                                                adminReportFileName,
                                                adminReport,
                                                out errorMessageCourseTransferUpdate);
            if (!string.IsNullOrEmpty(errorMessageCourseTransferUpdate)) Console.WriteLine("Error on CourseTransferUpdate" + errorMessageCourseTransferUpdate);

            Console.WriteLine("Migration completed.");
            string nextLine = Console.ReadLine();
        }

        internal static bool CheckForValidUKPRN(string ukprn)
        {
            string regex = "^[1][0-9]{7}$";
            var validUKPRN = Regex.Match(ukprn, regex, RegexOptions.IgnoreCase);

            return validUKPRN.Success;
        }

        internal static int GetCourseRunsCount(IEnumerable<CourseRun> courseRuns)
        {
            int countCourseRuns = 0;

            if (courseRuns != null)
            {
                using (IEnumerator<CourseRun> enumerator = courseRuns.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                        countCourseRuns++;
                }
            }

            return countCourseRuns;
        }


    }
}
