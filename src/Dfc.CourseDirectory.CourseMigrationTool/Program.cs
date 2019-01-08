using Dfc.CourseDirectory.Common.Interfaces;
using Dfc.CourseDirectory.Models.Interfaces.Courses;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Services.CourseService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration.GetConnectionString("DefaultConnection"));
            //var ss = configuration.GetSection("CourseServiceSettings");
            Console.WriteLine(configuration.GetValue<string>("name"));

            //Console.WriteLine($"Hello { config["name"] }!");
            Console.WriteLine("Please enter UKPRN:");
            string UKPRN = Console.ReadLine();
            Console.WriteLine(UKPRN);
            string nextLine2 = Console.ReadLine();
        }

        //private static async Task<IResult<ICourse>> AddCourseAsync(Course course)
        //{
        //    Logger logger = new Logger();
        //    ILogger<CourseService> logger = new ILogger<CourseService>();
        //    CourseService courseService = new CourseService();
        //    return await courseService.AddCourseAsync(course);
        //}
    }
}
