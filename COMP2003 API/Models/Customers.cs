using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace COMP2003_API.Models
{
    public partial class Customers
    {
        public Customers()
        {
            BookingAttendees = new HashSet<BookingAttendees>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContactNumber { get; set; }
        public string CustomerUsername { get; set; }
        public string CustomerPassword { get; set; }
        public virtual ICollection<BookingAttendees> BookingAttendees { get; set; }
    }
}
