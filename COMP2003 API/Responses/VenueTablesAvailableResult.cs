using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public class VenueTablesAvailableResult
    {
        public int TableId { get; set; }
        public int VenueTableNumber { get; set; }
        public int NumberOfSeats { get; set; }

    }
}
