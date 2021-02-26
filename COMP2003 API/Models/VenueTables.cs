using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace COMP2003_API.Models
{
    public partial class VenueTables
    {
        public VenueTables()
        {
            Bookings = new HashSet<Bookings>();
        }

        public int VenueTableId { get; set; }
        public int VenueId { get; set; }
        public int VenueTableNum { get; set; }
        public int VenueTableCapacity { get; set; }

        public virtual ICollection<Bookings> Bookings { get; set; }
    }
}
