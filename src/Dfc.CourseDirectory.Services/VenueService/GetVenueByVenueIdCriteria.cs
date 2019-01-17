using Dfc.CourseDirectory.Common;
using Dfc.CourseDirectory.Services.Interfaces.VenueService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.CourseDirectory.Services.VenueService
{
    public class GetVenueByVenueIdCriteria : ValueObject<GetVenueByVenueIdCriteria>, IGetVenueByVenueIdCriteria
    {
        public string venueId { get; set; }

        public GetVenueByVenueIdCriteria(string venueid)
        {
            //Throw.IfNull(venueid, nameof(venueid));
            Throw.IfNullOrEmpty(venueid, nameof(venueid));

            venueId = venueid;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return venueId;
        }
    }
}
