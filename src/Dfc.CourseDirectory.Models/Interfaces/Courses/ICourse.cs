﻿using Dfc.CourseDirectory.Models.Enums;
using Dfc.CourseDirectory.Models.Models.Courses;
using Dfc.CourseDirectory.Models.Models.Providers;
using Dfc.CourseDirectory.Models.Models.Qualifications;
using System;
using System.Collections.Generic;

namespace Dfc.CourseDirectory.Models.Interfaces.Courses
{
    public interface ICourse
    {
        Guid id { get; set; }
        string QualificationCourseTitle { get; set; } 
        string LearnAimRef { get; set; } 
        string NotionalNVQLevelv2 { get; set; } 
        string AwardOrgCode { get; set; } 
        string QualificationType { get; set; } 
        int ProviderUKPRN { get; set; } 
        string CourseDescription { get; set; }
        string EntryRequirements { get; set; } 
        string WhatYoullLearn { get; set; }
        string HowYoullLearn { get; set; }
        string WhatYoullNeed { get; set; }
        string HowYoullBeAssessed { get; set; }
        string WhereNext { get; set; }
        bool AdultEducationBudget { get; set; }
        bool AdvancedLearnerLoan { get; set; }
        IEnumerable<CourseRun> CourseRuns { get; }
        RecordStatus CourseStatus { get; }
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        DateTime? UpdatedDate { get; set; }
        string UpdatedBy { get; set; }

        bool IsValid { get; set; }
    }
}