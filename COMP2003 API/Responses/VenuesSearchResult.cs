using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public struct VenuesSearchResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
    }
}
