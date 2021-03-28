using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace COMP2003_API.Requests
{
    public struct DeletionRequest
    {
        [JsonRequired]
        public int Id { get; set; }
    }
}
