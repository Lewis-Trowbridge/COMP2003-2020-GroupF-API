using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public struct MinifiedBookingResult
    {
        public int BookingId { get; set; }
        public DateTime BookingDateTime { get; set; }
        public int BookingSize { get; set; }
        public string VenueName { get; set; }
        public string VenuePostcode { get; set; }
    }
}
