using Dfc.CourseDirectory.CourseMigrationTool.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Dfc.CourseDirectory.CourseMigrationTool.Tests
{
    public class CourseMigrationToolTests
    {
        [Fact]
        public void GetCoursesByProviderUKPRNTest()
        {
            int ExpectedCourseCount = 5;
            int providerUKPRN = 10009099;
            string connectionString = "Data Source=.;Database=SFA_CourseDirectory;Integrated Security=True;";
            string providerName = string.Empty;
            bool advancedLearnerLoan = false;
            string errorMessageGetCourses = string.Empty;
            var tribalCourses = DataHelper.GetCoursesByProviderUKPRN(providerUKPRN, connectionString, out providerName, out advancedLearnerLoan, out errorMessageGetCourses);

            Assert.Equal(ExpectedCourseCount, tribalCourses.Count);
        }
    }
}
