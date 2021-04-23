using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Responses;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    class VenuesControllerTestHelper
    {
        public static MinifiedVenueResult GetMinifiedVenueResult(Venues venue)
        {
            MinifiedVenueResult result = new MinifiedVenueResult
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                VenueCity = venue.City,
                VenuePostcode = venue.VenuePostcode
            };
            return result;
        }
    }
}
