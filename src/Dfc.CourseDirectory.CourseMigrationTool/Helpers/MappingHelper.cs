using Dfc.CourseDirectory.Models.Models.Courses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.CourseMigrationTool.Helpers
{
    public static class MappingHelper
    {
        public static Course MapTribalCourseToCourse(TribalCourse tribalCourse)
        {
            var course = new Course();
            var courseRuns = new List<CourseRun>();

            foreach (var tribalCourseRun in tribalCourse.TribalCourseRuns)
            {
                var courseRun = new CourseRun();
                courseRun.id = Guid.NewGuid();
                courseRun.CourseInstanceId = tribalCourseRun.CourseInstanceId;
                courseRun.VenueId = tribalCourseRun.VenueGuidId;
                courseRun.CourseName = tribalCourseRun.CourseName;
                courseRun.ProviderCourseID = tribalCourseRun.ProviderOwnCourseInstanceRef;
                //courseRun.DeliveryMode = tribalCourseRun.DeliveryMode,
                
                //courseRun.FlexibleStartDate = tribalCourseRun.flexibleStartDate,
                //courseRun.StartDate = tribalCourseRun.specifiedStartDate,
                if(tribalCourseRun.StartDate != null && tribalCourseRun.StartDate > DateTime.MinValue)
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
                
                //courseRun.DurationUnit = tribalCourseRun.Id,
                //courseRun.DurationValue = tribalCourseRun.DurationLength,

                // StudyMode 
                switch (tribalCourseRun.StudyMode)
                {
                    case TribalStudyMode.FullTime:
                        courseRun.StudyMode = StudyMode.FullTime;
                        break;
                    case TribalStudyMode.PartTime:
                        courseRun.StudyMode = StudyMode.PartTime;
                        break;
                    case TribalStudyMode.PartOfAFulltimeProgram:
                        //???????
                        courseRun.StudyMode = StudyMode.Undefined;
                        break;
                    case TribalStudyMode.Flexible:
                        courseRun.StudyMode = StudyMode.Flexible; // Here it looks identical but value changes from 4 to 3
                        break;
                    case TribalStudyMode.NotKnown:
                    case TribalStudyMode.Undefined:
                    default:
                        courseRun.StudyMode = StudyMode.Undefined;
                        break;
                }
                //courseRun.AttendancePattern = model.AttendanceMode,

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

            course.CreatedDate = DateTime.Now;
            course.CreatedBy = "DFC – Course Migration Tool";

            return course;
        }
    }
}
