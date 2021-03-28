using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public struct DeletionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
