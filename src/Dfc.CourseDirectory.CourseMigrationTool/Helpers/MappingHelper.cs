using Dfc.CourseDirectory.Models.Models.Courses;
using System;
using System.Collections.Generic;
using Dfc.CourseDirectory.Models.Enums;

namespace Dfc.CourseDirectory.CourseMigrationTool.Helpers
{
    public static class MappingHelper
    {
        public static Course MapTribalCourseToCourse(TribalCourse tribalCourse, out List<string> mappingMessages)
        {
            var course = new Course();
            var courseRuns = new List<CourseRun>();
            mappingMessages = new List<string>();

            foreach (var tribalCourseRun in tribalCourse.TribalCourseRuns)
            {
                var courseRun = new CourseRun();

                // JUST FOR TESTING - DO NOT UNCOMMENT
                tribalCourseRun.AttendanceType = AttendanceType.DistanceWithAttendance;
                tribalCourseRun.DurationUnit = TribalDurationUnit.Hours;
                tribalCourseRun.StudyMode = TribalStudyMode.PartOfAFulltimeProgram;
                tribalCourseRun.AttendancePattern = TribalAttendancePattern.Customised;

                // It's need it, because of the VenueId Check
                if (tribalCourseRun.RecordStatus.Equals(RecordStatus.Pending))
                {
                    courseRun.RecordStatus = RecordStatus.Pending;
                }
                else
                {
                    courseRun.RecordStatus = RecordStatus.Live;
                }
                
                courseRun.id = Guid.NewGuid();
                courseRun.CourseInstanceId = tribalCourseRun.CourseInstanceId;
                courseRun.VenueId = tribalCourseRun.VenueGuidId;
                courseRun.CourseName = tribalCourseRun.CourseName;
                courseRun.ProviderCourseID = tribalCourseRun.ProviderOwnCourseInstanceRef;

                // AttendanceType <=> DeliveryMode,
                switch (tribalCourseRun.AttendanceType)
                {
                    case AttendanceType.Location:
                        courseRun.DeliveryMode = DeliveryMode.ClassroomBased;
                        break;
                    case AttendanceType.FaceToFaceNonCampus:
                        courseRun.DeliveryMode = DeliveryMode.WorkBased;
                        break;
                    case AttendanceType.WorkBased:
                        courseRun.DeliveryMode = DeliveryMode.WorkBased; 
                        break;
                    case AttendanceType.OnlineWithoutAttendance:
                        courseRun.DeliveryMode = DeliveryMode.Online;
                        break;
                    case AttendanceType.MixedMode:
                    case AttendanceType.DistanceWithAttendance:
                    case AttendanceType.DistanceWithoutAttendance:                  
                    case AttendanceType.OnlineWithAttendance:
                    case AttendanceType.NotKnown:
                    case AttendanceType.Undefined:
                    default:
                        courseRun.DeliveryMode = DeliveryMode.Undefined;
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your AttendanceType/StudyMode is set to { tribalCourseRun.AttendanceType } and we don't have it" + Environment.NewLine);
                        break;
                }

                // StartDate & FlexibleStartDate
                if (tribalCourseRun.StartDate != null && tribalCourseRun.StartDate > DateTime.MinValue)
                {
                    courseRun.StartDate = tribalCourseRun.StartDate;
                    courseRun.FlexibleStartDate = false;
                }
                //else if (tribalCourseRun.StartDateDescription.Contains("Flexible", StringComparison.InvariantCultureIgnoreCase))
                //{
                //    courseRun.StartDate = null;
                //    courseRun.FlexibleStartDate = true;
                //}
                else
                {
                    courseRun.StartDate = null;
                    courseRun.FlexibleStartDate = true;
                }

                courseRun.CourseURL = tribalCourseRun.Url;
                courseRun.Cost = tribalCourseRun.Price;
                courseRun.CostDescription = tribalCourseRun.PriceAsText;

                // DurationUnit & DurationValue
                switch (tribalCourseRun.DurationUnit)
                {
                    case TribalDurationUnit.Hours:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Undefined;
                        // Alternativly calculation // TODO 
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your DurationUnit is set to { tribalCourseRun.DurationUnit } and we don't have it" +
                                            $"We preserved the DurationValue, but you have to set appropriate DurationUnit and change the DurationValue accordingly" + Environment.NewLine);
                        break;
                    case TribalDurationUnit.Days:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Days;
                        break;
                    case TribalDurationUnit.Weeks:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Weeks; 
                        break;
                    case TribalDurationUnit.Months:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Months;
                        break;
                    case TribalDurationUnit.Terms:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Undefined;
                        // Alternativly 3 x Months or X x Weeks // TODO 
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your DurationUnit is set to { tribalCourseRun.DurationUnit } and we don't have it" +
                                            $"We preserved the DurationValue, but you have to set appropriate DurationUnit and change the DurationValue accordingly" + Environment.NewLine);
                        break;
                    case TribalDurationUnit.Semesters:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Undefined;
                        // Alternativly 3 x Months or X x Weeks // TODO 
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your DurationUnit is set to { tribalCourseRun.DurationUnit } and we don't have it" +
                                            $"We preserved the DurationValue, but you have to set appropriate DurationUnit and change the DurationValue accordingly" + Environment.NewLine);
                        break;
                    case TribalDurationUnit.Years:
                        courseRun.DurationValue = tribalCourseRun.DurationValue;
                        courseRun.DurationUnit = DurationUnit.Years;
                        break;
                    case TribalDurationUnit.Undefined:
                    default:
                        courseRun.DurationUnit = DurationUnit.Undefined;
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your StudyMode is set to { tribalCourseRun.DurationUnit } and we don't have it" + Environment.NewLine);
                        break;
                }

                // StudyMode 
                switch (tribalCourseRun.StudyMode)
                {
                    case TribalStudyMode.FullTime:
                        courseRun.StudyMode = StudyMode.FullTime;
                        break;
                    case TribalStudyMode.PartTime:
                        courseRun.StudyMode = StudyMode.PartTime;
                        break;
                    case TribalStudyMode.Flexible:
                        courseRun.StudyMode = StudyMode.Flexible; // Here it looks identical but value changes from 4 to 3
                        break;
                    case TribalStudyMode.PartOfAFulltimeProgram:
                    case TribalStudyMode.NotKnown:
                    case TribalStudyMode.Undefined:
                    default:
                        courseRun.StudyMode = StudyMode.Undefined;
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING " +
                                            $"because your StudyMode is set to { tribalCourseRun.StudyMode } and we don't have it" + Environment.NewLine); 
                        break;
                }
                // AttendancePattern 
                switch (tribalCourseRun.AttendancePattern)
                {
                    case TribalAttendancePattern.DaytimeWorkingHours:
                        courseRun.AttendancePattern = AttendancePattern.Daytime;
                        break;
                    case TribalAttendancePattern.DayBlockRelease:
                        courseRun.AttendancePattern = AttendancePattern.DayOrBlockRelease; // Here it looks identical but value changes from 2 to 4
                        break;
                    case TribalAttendancePattern.Evening:
                    case TribalAttendancePattern.Twilight:
                        courseRun.AttendancePattern = AttendancePattern.Evening; 
                        break;
                    case TribalAttendancePattern.Weekend:
                        courseRun.AttendancePattern = AttendancePattern.Weekend; // Here it looks identical but value changes from 5 to 3
                        break;
                    case TribalAttendancePattern.Customised:
                    case TribalAttendancePattern.NotKnown:
                    case TribalAttendancePattern.NotApplicable:
                    case TribalAttendancePattern.Undefined:
                    default:
                        courseRun.AttendancePattern = AttendancePattern.Undefined;
                        courseRun.RecordStatus = RecordStatus.Pending;
                        mappingMessages.Add($"ATTENTION - CourseRun { tribalCourseRun.CourseInstanceId } with Ref: '{ tribalCourseRun.ProviderOwnCourseInstanceRef }' is set to PENDING, " +
                                            $"because your AttendancePattern is set to { tribalCourseRun.AttendancePattern } and we don't have it" + Environment.NewLine);
                        break;
                }

                courseRun.CreatedDate = DateTime.Now;
                courseRun.CreatedBy = "DFC – Course Migration Tool";

                courseRuns.Add(courseRun);
            }

            course.id = Guid.NewGuid();
            course.CourseId = tribalCourse.CourseId;
            course.QualificationCourseTitle = tribalCourse.CourseTitle;
            course.LearnAimRef = tribalCourse.LearningAimRefId;
            course.NotionalNVQLevelv2 = tribalCourse.QualificationLevelId.ToString();
            course.AwardOrgCode = tribalCourse.LearningAimAwardOrgCode;
            course.QualificationType = tribalCourse.Qualification;
            course.ProviderUKPRN = tribalCourse.Ukprn;
            course.CourseDescription = tribalCourse.CourseSummary;
            course.EntryRequirments = tribalCourse.EntryRequirements;
            course.WhatYoullLearn = tribalCourse.WhatYoullLearn;
            course.HowYoullLearn = tribalCourse.HowYoullLearn;
            course.WhatYoullNeed = tribalCourse.EquipmentRequired;
            course.HowYoullBeAssessed = tribalCourse.AssessmentMethod;
            course.WhereNext = tribalCourse.WhereNext;
            course.AdvancedLearnerLoan = tribalCourse.AdvancedLearnerLoan; 

            course.CourseRuns = courseRuns;

            // At least One CourseRun must be LIVE in order Course to be LIVE.
            if (courseRuns.Find(cr => cr.RecordStatus == RecordStatus.Live) != null)
            {
                course.RecordStatus = RecordStatus.Live;
            }
            else
            {
                course.RecordStatus = RecordStatus.Pending;
            }
            course.CreatedDate = DateTime.Now;
            course.CreatedBy = "DFC – Course Migration Tool";

            return course;
        }
    }
}
