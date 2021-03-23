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
    }
}
