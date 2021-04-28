using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Responses;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    class LoginControllerTestHelper
    {
        public static LoginResult GetSuccessfulLoginResult(Customers customer)
        {
            return new LoginResult
            {
                Success = true,
                Id = customer.CustomerId,
                Message = "Login successful."
            };
        }
    }
}
