using System;
using Xunit;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    public class COMP2003TestHelper
    {
        public static COMP2003_FContext GetDbContext()
        {
            // Configuration should occur through
            COMP2003_FContext dbContext = new COMP2003_FContext();
            return dbContext;
        }

        public static Customers GetTestCustomer(int id)
        {
            Customers customer = new Customers();
            customer.CustomerId = id;
            customer.CustomerName = "Test User";
            customer.CustomerContactNumber = "01984454191";
            customer.CustomerUsername = "TestUser";
            customer.CustomerPassword = "TestPassword";

            return customer;
        }
    }
}
