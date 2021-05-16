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
        private COMP2003_FContext dbContext;
        private VenuesController controller;

        public VenuesControllerTests()
        {
            dbContext = COMP2003TestHelper.GetDbContext();
            controller = new VenuesController(dbContext);
        }

        [Fact]
        public async void View_ValidVenue_ReturnsVenueInformation()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            Venues testVenue = COMP2003TestHelper.GetTestVenue(0);

            await dbContext.Venues.AddAsync(testVenue);
            await dbContext.SaveChangesAsync();

            // Act
            ActionResult<AppVenueView> actionResult = await controller.View(testVenue.VenueId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var result = (AppVenueView)okObjectResult.Value;

            // Assert
            // Since these are different types of objects, it is necessary to compare each value seperately
            Assert.Equal(testVenue.VenueId, result.VenueId);
            Assert.Equal(testVenue.VenueName, result.VenueName);
            Assert.Equal(testVenue.VenuePostcode, result.VenuePostcode);
            Assert.Equal(testVenue.AddLineOne, result.AddLineOne);
            Assert.Equal(testVenue.AddLineTwo, result.AddLineTwo);
            Assert.Equal(testVenue.City, result.City);
            Assert.Equal(testVenue.County, result.County);
        }

        [Fact]
        public async void View_NonexistentVenue_Fails()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            Venues testVenue = COMP2003TestHelper.GetTestVenue(0);

            await dbContext.Venues.AddAsync(testVenue);
            await dbContext.SaveChangesAsync();

            // Act
            ActionResult<AppVenueView> actionResult = await controller.View(testVenue.VenueId + 1);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void ViewTop_Returns_ListOfMinifiedVenueResults()
        {
            // Arrange

            // Act
            ActionResult<List<MinifiedVenueResult>> actionResult = await controller.ViewTop();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var result = (List<MinifiedVenueResult>)okObjectResult.Value;

            // Assert
            Assert.IsType<List<MinifiedVenueResult>>(result);

        }

        [Fact]
        public async void ViewTop_ReturnsOnlyTopVenues()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
           

            // Add 31 venues to ensure that there are more than 30 venues in the database
            for (int i = 0; i < 31; i++)
            {
                Venues testVenue = COMP2003TestHelper.GetTestVenue(0);
                await dbContext.AddAsync(testVenue);
            }

            await dbContext.SaveChangesAsync();

            // Act
            ActionResult<List<MinifiedVenueResult>> actionResult = await controller.ViewTop();
            var okObjectResult = actionResult.Result as OkObjectResult;
            var result = (List<MinifiedVenueResult>)okObjectResult.Value;

            // Assert
            Assert.True(result.Count == 30);
            
        }

        [Theory]
        [InlineData("Test venue")]
        [InlineData("PL4 8AA")]
        [InlineData("PL48AA")]
        [InlineData("Test city")]
        public async void Search_ForExistingVenue_ReturnsMatchingVenues(string searchString)
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            Venues testVenue = COMP2003TestHelper.GetTestVenue(0);
            await dbContext.AddAsync(testVenue);
            await dbContext.SaveChangesAsync();

            MinifiedVenueResult expectedResult = VenuesControllerTestHelper.GetMinifiedVenueResult(testVenue);

            // Act
            ActionResult<List<MinifiedVenueResult>> actionResult = await controller.Search(searchString);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realList = (List<MinifiedVenueResult>)okObjectResult.Value;


            // Assert
            Assert.Contains(expectedResult, realList);
        }

        [Theory]
        [InlineData("%^%//!")]
        [InlineData("WQ31 3RR")]
        public async void Search_ForNonexistentVenue_ReturnsEmptyList(string searchString)
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            Venues testVenue = COMP2003TestHelper.GetTestVenue(0);
            await dbContext.AddAsync(testVenue);
            await dbContext.SaveChangesAsync();

            MinifiedVenueResult expectedResult = VenuesControllerTestHelper.GetMinifiedVenueResult(testVenue);

            // Act
            ActionResult<List<MinifiedVenueResult>> actionResult = await controller.Search(searchString);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realList = (List<MinifiedVenueResult>)okObjectResult.Value;


            // Assert
            Assert.Empty(realList);
        }

        [Fact]
        public async void TablesAvailable_CorrectPartySizeAndTime()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            VenueTables testTable = await VenuesControllerTestHelper.InsertVenueTableSet(dbContext);
            VenueTablesAvailableResult testResult = VenuesControllerTestHelper.GetVenueTablesAvailableResult(testTable);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(testTable.VenueId, testTable.VenueTableCapacity, enteredTime);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (List<VenueTablesAvailableResult>)okObjectResult.Value;

            // Assert
            Assert.True(realResult.Count >= 1);
            Assert.Contains(testResult, realResult);
        }

        [Fact]
        public async void TablesAvailable_IncorrectPartySizeAndTime()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            VenueTables testTable = await VenuesControllerTestHelper.InsertVenueTableSet(dbContext);
            VenueTablesAvailableResult testResult = VenuesControllerTestHelper.GetVenueTablesAvailableResult(testTable);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(testTable.VenueId, testTable.VenueTableCapacity + 1, enteredTime);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (List<VenueTablesAvailableResult>)okObjectResult.Value;

            // Assert
            Assert.True(realResult.Count == 0);
            Assert.DoesNotContain(testResult, realResult);

        }

        [Fact]
        public async void TablesAvailable_IncorrectVenueId()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            VenueTables testTable = await VenuesControllerTestHelper.InsertVenueTableSet(dbContext);
            VenueTablesAvailableResult testResult = VenuesControllerTestHelper.GetVenueTablesAvailableResult(testTable);

            // Act
            DateTime enteredTime = DateTime.Parse("2701-12-25 15:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(9999, 4, enteredTime);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (List<VenueTablesAvailableResult>)okObjectResult.Value;

            // Assert
            Assert.True(realResult.Count == 0);
            Assert.DoesNotContain(testResult, realResult);
        }

        [Fact]
        public async void TablesAvailable_ReturnsType()
        {
            // Arrange

            // Act
            DateTime enteredTime = DateTime.Parse("2001-12-25 18:10:00");
            ActionResult<List<VenueTablesAvailableResult>> actionResult = await controller.TablesAvailable(1, 4, enteredTime);

            // Assert
            Assert.IsType<ActionResult<List<VenueTablesAvailableResult>>>(actionResult);
        }

    }
}
