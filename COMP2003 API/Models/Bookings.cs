using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace COMP2003_API.Models
{
    public partial class Bookings
    {
        public int BookingId { get; set; }
        public DateTime BookingTime { get; set; }
        public int BookingSize { get; set; }
        public int VenueId { get; set; }
        public int VenueTableId { get; set; }

        public virtual Venues Venue { get; set; }
        public virtual VenueTables VenueTable { get; set; }
    }
}
