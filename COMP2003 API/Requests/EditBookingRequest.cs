using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace COMP2003_API.Requests
{
    public class EditBookingRequest
    {
        [JsonRequired]
        public int BookingId { get; set; }
        [JsonProperty]
        public DateTime? BookingTime { get; set; }
        [JsonProperty]
        public int BookingSize { get; set; }
        [JsonProperty]
        public int VenueTableId { get; set; }
    }
}
