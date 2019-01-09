using Dfc.CourseDirectory.Models.Models.Courses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Dfc.CourseDirectory.CourseMigrationTool.Helpers
{
    public static class DataHelper
    {
        public static List<TribalCourse> GetCoursesByProviderUKPRN(int ProviderUKPRN, string connectionString, out string ProviderName)
        {
            var tribalCourses = new List<TribalCourse>();
            ProviderName = string.Empty;

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dfc_GetCoursesByProviderUKPRN";

                    command.Parameters.Add(new SqlParameter("@ProviderUKPRN", SqlDbType.Int));
                    command.Parameters["@ProviderUKPRN"].Value = ProviderUKPRN;

                    command.Parameters.Add(new SqlParameter("@ProviderName", SqlDbType.NVarChar, 200));
                    command.Parameters["@ProviderName"].Direction = ParameterDirection.Output;

                    try
                    {
                        //Open connection.
                        sqlConnection.Open();

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                TribalCourse tribalCourse = ExtractCourseFromDbReader(dataReader);
                                if (tribalCourse != null)
                                    tribalCourses.Add(tribalCourse);
                            }
                            // Close the SqlDataReader.
                            dataReader.Close();
                        }

                        // Get the Provider Name
                        ProviderName = (string)command.Parameters["@ProviderName"].Value;
                    }
                    catch(Exception ex)
                    {
                        string errorMessage = ex.Message;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }

            return tribalCourses;
        }

        public static TribalCourse ExtractCourseFromDbReader(SqlDataReader reader)
        {
            TribalCourse tribalCourse = new TribalCourse();

            tribalCourse.Ukprn = (int)CheckForDbNull(reader["Ukprn"], 0);
            tribalCourse.CourseId = (int)CheckForDbNull(reader["CourseId"], 0);
            tribalCourse.CourseTitle = (string)CheckForDbNull(reader["CourseTitle"], string.Empty);
            tribalCourse.LearningAimRefId = (string)CheckForDbNull(reader["LearningAimRefId"], string.Empty);
            tribalCourse.QualificationLevelId = (int)CheckForDbNull(reader["QualificationLevelId"], 0);
            tribalCourse.LearningAimAwardOrgCode = (string)CheckForDbNull(reader["LearningAimAwardOrgCode"], string.Empty);
            tribalCourse.Qualification = (string)CheckForDbNull(reader["Qualification"], string.Empty);
            tribalCourse.CourseSummary = (string)CheckForDbNull(reader["CourseSummary"], string.Empty);
            tribalCourse.EntryRequirements = (string)CheckForDbNull(reader["EntryRequirements"], string.Empty);
            ////
            tribalCourse.AssessmentMethod = (string)CheckForDbNull(reader["AssessmentMethod"], string.Empty);
            ////

            return tribalCourse;
        }

        public static object CheckForDbNull(object valueToCheck, object replacementValue)
        {
            return valueToCheck == DBNull.Value ? replacementValue : valueToCheck;
        }
    }
}
