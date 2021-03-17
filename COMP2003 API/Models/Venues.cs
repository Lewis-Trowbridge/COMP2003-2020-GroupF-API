using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace COMP2003_API.Models
{
    public partial class Venues
    {
        public Venues()
        {
            Bookings = new HashSet<Bookings>();
            VenueTables = new HashSet<VenueTables>();
        }

        public int VenueId { get; set; }
        public string VenueName { get; set; }
        public string VenuePostcode { get; set; }
        public string AddLineOne { get; set; }
        public string AddLineTwo { get; set; }
        public string City { get; set; }
        public string County { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }
        public virtual ICollection<VenueTables> VenueTables { get; set; }
    }
}
