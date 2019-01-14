using Dfc.CourseDirectory.Common.Interfaces;
using Dfc.CourseDirectory.CourseMigrationTool.Helpers;
using Dfc.CourseDirectory.Models.Interfaces.Courses;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Services.CourseService;
using Dfc.CourseDirectory.Services.Interfaces.CourseService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
//using Microsoft.Extensions.Logging.Console;
//using Microsoft.Extensions.Options.ConfigurationExtensions;

namespace Dfc.CourseDirectory.CourseMigrationTool
{
    class Program
    {
        static void Main(string[] args)
        {

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json");

            //IConfiguration config = new ConfigurationBuilder()
            //    .AddJsonFile("appsettings.json", true, true)
            //    .Build();

            /* // That is working */
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();


            /*
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<ICourseService, CourseService>()
            .BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogDebug("Starting application");

            //do the actual work here
            var bar = serviceProvider.GetService<ICourseService>();

            Course course = new Course();
            bar.AddCourseAsync(course);

            logger.LogDebug("All done!");
            */


            // Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));         
            //Console.WriteLine(configuration.GetValue<string>("name"));

            /*
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<Application>();
            Task.Run(() => app.Run()).Wait();
            */

            string connectionString = configuration.GetConnectionString("DefaultConnection");
            bool generateFilesLocally = configuration.GetValue<bool>("GenerateFilesLocally");
            string jsonCourseFilesPath = configuration.GetValue<string>("JsonCourseFilesPath");

            Console.WriteLine("Please enter UKPRN:");
            string UKPRN = Console.ReadLine();

            if (!CheckForValidUKPRN(UKPRN))
            {
                Console.WriteLine("UKPRN must be 8 digit number starting with a 1 e.g. 10000364");
                UKPRN = Console.ReadLine();
            }

            int providerUKPRN = Convert.ToInt32(UKPRN);

            string providerName = string.Empty;
            var tribalCourses = DataHelper.GetCoursesByProviderUKPRN(providerUKPRN, connectionString, out providerName);

            foreach(var tribalCourse in tribalCourses)
            {
                var tribalCourseRuns = DataHelper.GetCourseInstancesByCourseId(tribalCourse.CourseId, connectionString);

                if (tribalCourseRuns != null)
                {
                    tribalCourse.TribalCourseRuns = tribalCourseRuns;
                    foreach(var tribalCourseRun in tribalCourse.TribalCourseRuns)
                    {
                        tribalCourseRun.CourseName = tribalCourse.CourseTitle;
                        // Call VenueService and for each tribalCourseRun.VenueId get tribalCourseRun.VenueGuidId
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

            Console.WriteLine(providerName);
            string nextLine = Console.ReadLine();

        }

        internal static bool CheckForValidUKPRN(string ukprn)
        {
            string regex = "^[1][0-9]{7}$";
            var validUKPRN = Regex.Match(ukprn, regex, RegexOptions.IgnoreCase);

            return validUKPRN.Success;
        }
        /*
        private static void ConfigureServices(IServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole() // Error!
                .AddDebug();

            services.AddSingleton(loggerFactory); // Add first my already configured instance
            services.AddLogging(); // Allow ILogger<T>

            IConfigurationRoot configuration = GetConfiguration();
            services.AddSingleton<IConfigurationRoot>(configuration);

            // Support typed Options
            services.AddOptions();
            services.Configure<MyOptions>(configuration.GetSection("MyOptions")); // Error!

            services.AddTransient<Application>();
        }

        private static IConfigurationRoot GetConfiguration()
        {
            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        }

        public class MyOptions
        {
            public string Name { get; set; }
        }

        public class Application
        {
            ILogger _logger;
            MyOptions _settings;

            public Application(ILogger<Application> logger, IOptions<MyOptions> settings)
            {
                _logger = logger;
                _settings = settings.Value;
            }

            public async Task Run()
            {
                try
                {
                    _logger.LogInformation($"This is a console application for {_settings.Name}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
        */

        //private static async Task<IResult<ICourse>> AddCourseAsync(Course course)
        //{
        //    Logger logger = new Logger();
        //    ILogger<CourseService> logger = new ILogger<CourseService>();
        //    CourseService courseService = new CourseService();
        //    return await courseService.AddCourseAsync(course);
        //}
    }
}
