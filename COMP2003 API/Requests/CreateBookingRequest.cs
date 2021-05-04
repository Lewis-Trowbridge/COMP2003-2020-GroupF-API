using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace COMP2003_API.Requests
{
    public struct CreateBookingRequest
    {
        [JsonRequired]
        public int VenueTableId { get; set; }
        [JsonRequired]
        public int CustomerId { get; set; }
        [JsonRequired]
        public DateTime BookingDateTime { get; set; }
        [JsonRequired]
        public int BookingSize { get; set; }
    }
}
