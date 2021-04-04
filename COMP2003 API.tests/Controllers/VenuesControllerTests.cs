using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Responses;
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

        [Fact]
        public async void TablesAvailable_CorrectPartySizeAndTime()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            VenuesController controller = new VenuesController(dbContext);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(1, 4, enteredTime);

            // Assert
            Assert.True(actionResult.Value.Count >= 1);
        }

        [Fact]
        public async void TablesAvailable_IncorrectPartySizeAndTime()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            VenuesController controller = new VenuesController(dbContext);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(1, 77, enteredTime);

            // Assert
            Assert.True(actionResult.Value.Count == 0);
        }

        [Fact]
        public async void TablesAvailable_IncorrectVenueId()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            VenuesController controller = new VenuesController(dbContext);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(9999, 4, enteredTime);

            // Assert
            Assert.True(actionResult.Value.Count == 0);
        }

        [Fact]
        public async void TablesAvailable_ReturnsType()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            VenuesController controller = new VenuesController(dbContext);            

            // Act
            DateTime enteredTime = DateTime.Parse("2001-12-25 18:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(1, 4, enteredTime);

            // Assert
            Assert.IsType<ActionResult<List<VenueTablesAvailableResult>>>(actionResult);
        }

    }
}
