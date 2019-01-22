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
        public static List<TribalCourse> GetCoursesByProviderUKPRN(int ProviderUKPRN, string connectionString, out string ProviderName, out bool AdvancedLearnerLoan, out string errorMessageGetCourses)
        {
            var tribalCourses = new List<TribalCourse>();
            ProviderName = string.Empty;
            AdvancedLearnerLoan = false;
            errorMessageGetCourses = string.Empty;

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

                    command.Parameters.Add(new SqlParameter("@AdvancedLearnerLoan", SqlDbType.Bit));
                    command.Parameters["@AdvancedLearnerLoan"].Direction = ParameterDirection.Output;

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
                        // Get the AdvancedLearnerLoan
                        AdvancedLearnerLoan = (bool)command.Parameters["@AdvancedLearnerLoan"].Value;
                    }
                    catch(Exception ex)
                    {
                        errorMessageGetCourses = string.Format("Error Message: {0}" + Environment.NewLine + "Stack Trace: {1}", ex.Message, ex.StackTrace);
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
            tribalCourse.CourseTitle = (string)CheckForDbNull(reader["LearningAimTitle"], string.Empty);
            tribalCourse.LearningAimRefId = (string)CheckForDbNull(reader["LearningAimRefId"], string.Empty);
            tribalCourse.QualificationLevelId = (int)CheckForDbNull(reader["QualificationLevelId"], 0);
            tribalCourse.LearningAimAwardOrgCode = (string)CheckForDbNull(reader["LearningAimAwardOrgCode"], string.Empty);
            tribalCourse.Qualification = (string)CheckForDbNull(reader["Qualification"], string.Empty);
            tribalCourse.CourseSummary = (string)CheckForDbNull(reader["CourseSummary"], string.Empty);
            tribalCourse.EntryRequirements = (string)CheckForDbNull(reader["EntryRequirements"], string.Empty);
            tribalCourse.AssessmentMethod = (string)CheckForDbNull(reader["AssessmentMethod"], string.Empty);
            ////
            //tribalCourse.AdvancedLearnerLoan = // TODO:
            return tribalCourse;
        }

        public static List<TribalCourseRun> GetCourseInstancesByCourseId(int CourseId, string connectionString, out string errorMessageGetCourseRuns)
        {
            errorMessageGetCourseRuns = string.Empty;
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
                        errorMessageGetCourseRuns = string.Format("Error Message: {0}" + Environment.NewLine + "Stack Trace: {1}", ex.Message, ex.StackTrace);
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

            tribalCourseRun.CourseId = (int)CheckForDbNull(reader["CourseId"], 0);
            tribalCourseRun.VenueId = (int?)CheckForDbNull(reader["VenueId"], null);
            tribalCourseRun.CourseInstanceId = (int)CheckForDbNull(reader["CourseInstanceId"], 0);
            tribalCourseRun.ProviderOwnCourseInstanceRef = (string)CheckForDbNull(reader["ProviderOwnCourseInstanceRef"], string.Empty);
            tribalCourseRun.AttendanceType = (AttendanceType)CheckForDbNull(reader["AttendanceTypeId"], AttendanceType.Undefined);
            tribalCourseRun.StartDateDescription = (string)CheckForDbNull(reader["StartDateDescription"], string.Empty);
            tribalCourseRun.StartDate = (DateTime?)CheckForDbNull(reader["StartDate"], null);
            tribalCourseRun.Url = (string)CheckForDbNull(reader["Url"], string.Empty);
            tribalCourseRun.Price = (decimal?)CheckForDbNull(reader["Price"], null);
            tribalCourseRun.PriceAsText = (string)CheckForDbNull(reader["PriceAsText"], string.Empty);
            tribalCourseRun.DurationUnit = (TribalDurationUnit)CheckForDbNull(reader["DurationUnitId"], TribalDurationUnit.Undefined);
            tribalCourseRun.DurationValue = (int)CheckForDbNull(reader["DurationUnit"], 0);
            tribalCourseRun.StudyMode = (TribalStudyMode)CheckForDbNull(reader["StudyModeId"], TribalStudyMode.Undefined);
            tribalCourseRun.AttendancePattern = (TribalAttendancePattern)CheckForDbNull(reader["AttendanceTypeId"], TribalAttendancePattern.Undefined);

            return tribalCourseRun;
        }

        public static object CheckForDbNull(object valueToCheck, object replacementValue)
        {
            return valueToCheck == DBNull.Value ? replacementValue : valueToCheck;
        }
    }
}
