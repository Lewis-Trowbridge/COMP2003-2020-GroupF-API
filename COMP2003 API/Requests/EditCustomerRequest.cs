using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace COMP2003_API.Requests
{
    public class EditCustomerRequest
    {
        [JsonRequired]
        public int CustomerId { get; set; }
        [JsonProperty]
        public string CustomerName { get; set; }
        [JsonProperty]
        public string CustomerContactNumber { get; set; }
        [JsonProperty]
        public string CustomerUsername { get; set; }
        [JsonProperty]
        public string CustomerPassword { get; set; }

    }
}
