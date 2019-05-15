using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.ApprenticeshipMigrationTool.Interfaces
{
    public interface ITribalProvider
    {
        string UnitedKingdomProviderReferenceNumber { get; set; }
        string ProviderName { get; set; }
        string ProviderNameAlias { get; set; }
        int? ProviderId { get; set; }
        int? UPIN { get; set; }
        string TradingName { get; set; }
        bool NationalApprenticeshipProvider { get; set; }
        string MarketingInformation { get; set; }
        string Email { get; set; }
        string Telephone { get; set; }
        string Website { get; set; }
    }
}
