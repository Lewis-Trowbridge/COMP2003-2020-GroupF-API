using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

#nullable enable

namespace COMP2003_API.Requests
{
    public class CreateBooking
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
