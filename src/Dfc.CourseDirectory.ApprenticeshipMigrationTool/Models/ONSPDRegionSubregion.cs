using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool.Models
{
    public class ONSPDRegionSubregion : IONSPDRegionSubregion
    {
        public string Postcode { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
    }
}
