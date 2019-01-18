using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Dfc.CourseDirectory.Models.Enums
{
    public enum RecordStatus
    {
        [Description("Undefined")]
        Undefined = 0,
        [Description("Pending")]
        Pending = 1,
        [Description("Live")]
        Live = 2,
        [Description("Archived")]
        Archived = 3,
        [Description("Deleted")]
        Deleted = 4
    }

    public class Enums
    {
    }
}
