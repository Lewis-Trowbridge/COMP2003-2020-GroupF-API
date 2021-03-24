using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Tests.Helpers;

namespace COMP2003_API.Tests.Controllers
{
    public class VenuesControllerTests
    {
        [Fact]
        public async void ViewTop_Returns_ListOfAppVenueViews()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            VenuesController controller = new VenuesController(dbContext);

            // Act
            ActionResult<List<AppVenueView>> actionResult = await controller.ViewTop();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var result = (List<AppVenueView>)okObjectResult.Value;

            // Assert
            Assert.IsType<List<AppVenueView>>(result);

        }

    }
}
