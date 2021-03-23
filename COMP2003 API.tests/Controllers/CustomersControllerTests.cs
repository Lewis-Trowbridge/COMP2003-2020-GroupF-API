using System;
using Xunit;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Responses;
using COMP2003_API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace COMP2003_API.Tests.Controllers
{
    public class CustomersControllerTests
    {
        [Fact]
        public async void Create_ReturnsBadResult_ForNullParameters()
        {
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            controller.ModelState.AddModelError("error", "Invalid Model parameters");

            ActionResult<CreationResult> result = await controller.Create(new Customers());

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }
    }
}
