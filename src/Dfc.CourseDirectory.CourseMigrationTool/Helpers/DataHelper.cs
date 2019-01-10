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

        public static List<TribalCourseRun> GetCourseInstancesByCourseId(int CourseId, string connectionString)
        {
            var tribalCourseRuns = new List<TribalCourseRun>();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var command = sqlConnection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dfc_GetCourseInstancesByCourseId";

                    command.Parameters.Add(new SqlParameter("@CourseId", SqlDbType.Int));
                    command.Parameters["@CourseId"].Value = CourseId;
                    
                    try
                    {
                        //Open connection.
                        sqlConnection.Open();

                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            while (dataReader.Read())
                            {
                                TribalCourseRun tribalCourseRun = ExtractCourseRunFromDbReader(dataReader);
                                if (tribalCourseRun != null)
                                    tribalCourseRuns.Add(tribalCourseRun);
                            }
                            // Close the SqlDataReader.
                            dataReader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.Message;
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }

            return tribalCourseRuns;
        }

        public static TribalCourseRun ExtractCourseRunFromDbReader(SqlDataReader reader)
        {
            TribalCourseRun tribalCourseRun = new TribalCourseRun();

            tribalCourseRun.VenueId = (int)CheckForDbNull(reader["VenueId"], 0);
            tribalCourseRun.CourseInstanceId = (int)CheckForDbNull(reader["CourseInstanceId"], 0);
            //tribalCourseRun.CourseId = (int)CheckForDbNull(reader["CourseId"], 0);
            tribalCourseRun.AttendanceType = (AttendanceType)CheckForDbNull(reader["AttendanceTypeId"], AttendanceType.Undefined);
            tribalCourseRun.StartDateDescription = (string)CheckForDbNull(reader["StartDateDescription"], string.Empty);
            tribalCourseRun.StartDate = (DateTime?)reader["StartDate"];
            tribalCourseRun.Url = (string)CheckForDbNull(reader["Url"], string.Empty);
            //tribalCourseRun.Qualification = (string)CheckForDbNull(reader["Qualification"], string.Empty);
            //tribalCourseRun.CourseSummary = (string)CheckForDbNull(reader["CourseSummary"], string.Empty);
            //tribalCourseRun.EntryRequirements = (string)CheckForDbNull(reader["EntryRequirements"], string.Empty);
            //////
            //tribalCourseRun.AssessmentMethod = (string)CheckForDbNull(reader["AssessmentMethod"], string.Empty);
            //////

            return tribalCourseRun;
        }

        public static object CheckForDbNull(object valueToCheck, object replacementValue)
        {
            return valueToCheck == DBNull.Value ? replacementValue : valueToCheck;
        }
    }
}
