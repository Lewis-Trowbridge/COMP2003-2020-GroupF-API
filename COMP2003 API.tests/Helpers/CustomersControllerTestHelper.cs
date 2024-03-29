﻿using System;
using System.Collections.Generic;
using System.Text;
using COMP2003_API.Models;
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

        public static CreationResult GetInvalidContactNumberResult()
        {
            CreationResult result = new CreationResult
            {
                Success = false,
                Message = "A validation error has occured with the submitted contact number."
            };
            return result;
        }

        public static CreateCustomerRequest GetTestCreateCustomer()
        {
            CreateCustomerRequest customer = new CreateCustomerRequest
            {
                CustomerName = "Test User",
                CustomerContactNumber = "+441984454191",
                CustomerUsername = "TestUser",
                CustomerPassword = "TestPassword"
            };
            return customer;
        }

        public static MinifiedCustomerResult GetMinifiedCustomerResult(Customers customer)
        {
            MinifiedCustomerResult result = new MinifiedCustomerResult
            {
                CustomerId = customer.CustomerId,
                CustomerContactNumber = customer.CustomerContactNumber,
                CustomerName = customer.CustomerName,
                CustomerUsername = customer.CustomerUsername
            };
            return result;
        }
    }
}
