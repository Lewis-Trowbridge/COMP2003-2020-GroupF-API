using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace COMP2003_API.Requests
{
    public class LoginRequest
    {
        [JsonRequired]
        public string CustomerUsername { get; set; }
        [JsonRequired]
        public string CustomerPassword { get; set; }
    }
}
