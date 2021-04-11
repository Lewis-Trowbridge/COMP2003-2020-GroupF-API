using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Requests;
using COMP2003_API.Responses;

namespace COMP2003_API.Tests.Helpers
{
    class CustomersControllerTestHelper
    {

        public static CreationResult GetSuccessfulCreationResult()
        {
            CreationResult result = new CreationResult
            {
                Success = true,
                Message = "A customer account has been created."
            };
            return result;
        }

        public static CreationResult GetAlreadyCreatedResult()
        {
            CreationResult result = new CreationResult
            {
                Success = false,
                Message = "An account with that username already exists."
            };
            return result;
        }

        public static CreateCustomer GetTestCreateCustomer()
        {
            CreateCustomer customer = new CreateCustomer
            {
                CustomerName = "Test User",
                CustomerContactNumber = "01984454191",
                CustomerUsername = "TestUser",
                CustomerPassword = "TestPassword"
            };
            return customer;
        }
    }
}
