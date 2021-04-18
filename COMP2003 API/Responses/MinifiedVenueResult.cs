using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public struct MinifiedVenueResult
    {
        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string VenueCity { get; set; }
        public string VenuePostcode { get; set; }
    }
}
