using Dfc.CourseDirectory.CourseMigrationTool.Interfaces;


namespace Dfc.CourseDirectory.CourseMigrationTool.Models
{
    public class LarsDataResultItem : ILarsDataResultItem
    {
        public string LearnAimRef { get; set; }
        public string LearnAimRefTitle { get; set; }
        public string NotionalNVQLevelv2 { get; set; }
        public string AwardOrgCode { get; set; }
        public string LearnAimRefTypeDesc { get; set; }
    }
}
