using Dfc.CourseDirectory.ApprenticeshipMigrationTool.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool.Models
{
    public class TribalProvider : ITribalProvider
    {
        public string UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
        public string ProviderNameAlias { get; set; }
        public int? ProviderId { get; set; }
        public int? UPIN { get; set; }        
        public string TradingName { get; set; }
        public bool NationalApprenticeshipProvider { get; set; }
        public string MarketingInformation { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Website { get; set; }
    }
}
