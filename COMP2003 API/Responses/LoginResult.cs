using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COMP2003_API.Responses
{
    public struct LoginResult
    {
        public bool Success { get; set; }
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
