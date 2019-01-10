using System;
using Dfc.CourseDirectory.Models.Interfaces.Courses;
using System.ComponentModel;

namespace Dfc.CourseDirectory.Models.Models.Courses
{
    //public enum DeliveryMode
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,
    //    [Description("Classroom based")]
    //    ClassroomBased = 1,
    //    [Description("Online")]
    //    Online = 2,
    //    [Description("Work based")]
    //    WorkBased = 3
    //}

    public enum AttendanceType
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Location / campus")]
        Location = 1,
        [Description("Face-to-face (non-campus)")]
        FaceToFaceNonCampus = 2,
        [Description("Work-based")]
        WorkBased = 3,
        [Description("Mixed mode")]
        MixedMode = 4,
        [Description("Distance with attendance")]
        DistanceWithAttendance = 5,
        [Description("Distance without attendance")]
        DistanceWithoutAttendance = 6,
        [Description("Online without attendance")]
        OnlineWithoutAttendance = 7,
        [Description("Online with attendance")]
        OnlineWithAttendance = 8,
        [Description("Not known")]
        NotKnown = 9
    }


    //public enum DurationUnit
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,
    //    [Description("Days")]
    //    Days = 1,
    //    [Description("Weeks")]
    //    Weeks = 2,
    //    [Description("Months")]
    //    Months = 3,
    //    [Description("Years")]
    //    Years = 4
    //}
    //public enum StudyMode
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,
    //    [Description("Full-time")]
    //    FullTime = 1,
    //    [Description("Part-time")]
    //    PartTime = 2,
    //    [Description("Flexible")]
    //    Flexible = 3
    //}

    //public enum AttendancePattern
    //{
    //    [Description("Undefined")]
    //    Undefined = 0,
    //    [Description("Daytime")]
    //    Daytime = 1,
    //    [Description("Evening")]
    //    Evening = 2,
    //    [Description("Weekend")]
    //    Weekend = 3,
    //    [Description("Day/Block Release")]
    //    DayOrBlockRelease = 4
    //}
    //public enum StartDateType
    //{
    //    [Description("Defined Start Date")]
    //    SpecifiedStartDate = 1,
    //    [Description("Select a flexible start date")]
    //    FlexibleStartDate = 2,
    //}

    public class TribalCourseRun : ITribalCourseRun
    {
        //public Guid id { get; set; }
        public int VenueId { get; set; } //=>  Call VenueService to get [VenueId](GUID) using [VenueLocationId] => TODO

        // public string CourseName { get; set; } => it will use CourseTitle from Course properties
        public int CourseInstanceId { get; set; } //=> ProviderCourseID ??? [ProviderOwnCourseInstanceRef] instead of CourseInstanceId
        public AttendanceType AttendanceType { get; set; } //=> DeliveryMode DeliveryMode
        public string StartDateDescription { get; set; } //=> FlexibleStartDate
        // Flexible start date - please just contact the programme to sign up
        // Flexible start dates throughout the year to suit the client / individual 
        // 5488 distinct values (358 of which contain the word 'Flexible')
        // => public bool FlexibleStartDate { get; set; }  // The course starts on 19/9/18
        public DateTime? StartDate { get; set; } //=> StartDate
        public string Url { get; set; } // => CourseURL
        public decimal Cost { get; set; } 
        public string CostDescription { get; set; }
        public DurationUnit DurationUnit { get; set; }
        public int DurationValue { get; set; }
        public StudyMode StudyMode { get; set; }
        public AttendancePattern AttendancePattern { get; set; }

        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}