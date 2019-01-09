using Dfc.CourseDirectory.Models.Interfaces.Courses;
using System;
using System.Collections.Generic;

namespace Dfc.CourseDirectory.Models.Models.Courses
{
    public class TribalCourse : ITribalCourse 
    {
        public int CourseId { get; set; } // Used to get TribalCourseRuns
        public string CourseTitle  { get; set; } // QualificationCourseTitle
        public string LearningAimRefId  { get; set; } // LearnAimRef => LARS => check 54007 = NULL empty ???
        public int QualificationLevelId { get; set; } // NotionalNVQLevelv2
        public string LearningAimAwardOrgCode { get; set; } // AwardOrgCode
        public string Qualification { get; set; } // QualificationType => ??? some of them empty NULL

        public int Ukprn { get; set; } // ProviderUKPRN

        public string CourseSummary { get; set; } // CourseDescription
        public string EntryRequirements { get; set; } // EntryRequirments
        public string WhatYoullLearn { get; set; } // ??? TBC
        public string HowYoullLearn { get; set; } // ??? TBC
        public string WhatYoullNeed { get; set; } // ??? TBC
        public string AssessmentMethod  { get; set; } // HowYoullBeAssessed
        public string EquipmentRequired  { get; set; } // WhereNext ??? That can NOT be true ???

        public bool AdvancedLearnerLoan { get; set; } // ??? NOT done
      
        public IEnumerable<TribalCourseRun> TribalCourseRuns { get; set; }
    }
}