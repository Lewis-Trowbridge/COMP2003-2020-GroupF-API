using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace COMP2003_API.Requests
{
    public class CreateBooking
    {
        [Required]
        public int VenueTableId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public DateTime BookingDateTime { get; set; }
        [Required]
        public int BookingSize { get; set; }
    }
}
