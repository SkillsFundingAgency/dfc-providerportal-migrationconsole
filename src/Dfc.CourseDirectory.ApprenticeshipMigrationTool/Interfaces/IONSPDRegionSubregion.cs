using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool.Interfaces
{
    public interface IONSPDRegionSubregion
    {
        string Postcode { get; set; }
        string Region { get; set; }
        string SubRegion { get; set; }
    }
}
