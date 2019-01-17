using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.CourseMigrationTool.Interfaces
{
    public interface IViewComponentModel
    {
        bool HasErrors { get; }
        IEnumerable<string> Errors { get; }
    }
}
