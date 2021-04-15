using System;
using Xunit;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using COMP2003_API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace COMP2003_API.Tests.Controllers
{
    public class CustomersControllerTests
    {

        [Fact]
        public async void Create_WithValidInputs_AddsSuccessfully()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            CustomersController controller = new CustomersController(dbContext);
            CreationResult expectedResult = CustomersControllerTestHelper.GetSuccessfulCreationResult();
            CreateCustomer creationRequest = CustomersControllerTestHelper.GetTestCreateCustomer();

            // Act
            ActionResult<CreationResult> actionResult = await controller.Create(creationRequest);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (CreationResult)okObjectResult.Value;

            Customers createdCustomer = dbContext.Customers.Find(realResult.Id);

            Assert.Contains(createdCustomer, dbContext.Customers);
            Assert.Equal(creationRequest.CustomerName, createdCustomer.CustomerName);
            Assert.Equal(creationRequest.CustomerContactNumber, createdCustomer.CustomerContactNumber);
            Assert.Equal(creationRequest.CustomerUsername, createdCustomer.CustomerUsername);
            // Password should be hashed, so they should not be equal
            Assert.NotEqual(creationRequest.CustomerPassword, createdCustomer.CustomerPassword);

            // As it is not possible to know the ID before it's created this cannot be tested for but
            // the ID should not be 0 if the creation has gone to plan
            Assert.NotEqual(0, realResult.Id);
            Assert.Equal(expectedResult.Success, realResult.Success);
            Assert.Equal(expectedResult.Message, realResult.Message);


        }

        [Fact]
        public async void Create_ExistingUser_Fails()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            CreationResult expectedResult = CustomersControllerTestHelper.GetAlreadyCreatedResult();
            CreateCustomer creationRequest = CustomersControllerTestHelper.GetTestCreateCustomer();
            Customers createdCustomer = COMP2003TestHelper.GetTestCustomer(0);

            // Insert customer in order to trigger already existing error
            await dbContext.Customers.AddAsync(createdCustomer);
            await dbContext.SaveChangesAsync();

            // Act
            ActionResult<CreationResult> actionResult = await controller.Create(creationRequest);
            var objectResult = actionResult.Result as ObjectResult;
            var realResult = (CreationResult)objectResult.Value;

            // Assert
            Assert.Equal(208, objectResult.StatusCode);
            Assert.Equal(expectedResult, realResult);

            // Cleanup
            dbContext.Remove(createdCustomer);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void Create_WithMissingInputs_Fails()
        {
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            controller.ModelState.AddModelError("error", "Invalid Model parameters");

            ActionResult<CreationResult> result = await controller.Create(new CreateCustomer());

            Assert.IsType<BadRequestObjectResult>(result.Result);

        }

        [Theory]
        [InlineData("310298309183091904731908")]
        [InlineData("2")]
        public async void Create_WithInvalidPhoneNumber_Fails(string phoneNumber)
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            CreationResult expectedResult = CustomersControllerTestHelper.GetInvalidContactNumberResult();
            CreateCustomer creationRequest = CustomersControllerTestHelper.GetTestCreateCustomer();
            creationRequest.CustomerContactNumber = phoneNumber;

            // Act
            ActionResult<CreationResult> actionResult = await controller.Create(creationRequest);
            var objectResult = actionResult.Result as BadRequestObjectResult;
            var realResult = (CreationResult)objectResult.Value;

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(expectedResult, realResult);
        }

        [Fact]
        public async void View_ValidUser_ReturnsUserInformation()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            Customers testCustomer = COMP2003TestHelper.GetTestCustomer(0);

            await dbContext.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            MinifiedCustomerResult testResult = CustomersControllerTestHelper.GetMinifiedCustomerResult(testCustomer);

            // Act
            ActionResult<MinifiedCustomerResult> actionResult = await controller.View(testCustomer.CustomerId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (MinifiedCustomerResult)okObjectResult.Value;

            // Assert
            Assert.Equal(testResult, realResult);
        }

        [Fact]
        public async void View_NonexistentUser_Fails()
        {
            // Arrange
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            Customers testCustomer = COMP2003TestHelper.GetTestCustomer(0);

            await dbContext.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            // Act
            ActionResult<MinifiedCustomerResult> actionResult = await controller.View(testCustomer.CustomerId + 1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
        public async void Delete_ExistingUser_DeletesSuccessfully()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new CustomersController(dbContext);
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            var expectedDeletionResult = ResponseTestHelper.GetSuccessfulAcccountDeletionResult();

            await dbContext.Customers.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            // Act
            var actionResult = await controller.Delete(testCustomer.CustomerId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realDeletionResult = (DeletionResult)okObjectResult.Value;


            // Assert
            Assert.DoesNotContain(testCustomer, dbContext.Customers);
            Assert.Equal(expectedDeletionResult, realDeletionResult);

        }

        [Fact]
        public async void Delete_NonexistentUser_Fails()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new CustomersController(dbContext);
            var testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            var expectedDeletionResult = ResponseTestHelper.GetFailedAccountDeletionResult();
            await dbContext.Customers.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            // Act
            var actionResult = await controller.Delete(testCustomer.CustomerId + 1);
            var notFoundObjectResult = actionResult.Result as NotFoundObjectResult;
            var realDeletionResult = (DeletionResult)notFoundObjectResult.Value;

            // Assert

            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(expectedDeletionResult, realDeletionResult);

            // Cleanup
            dbContext.Customers.Remove(testCustomer);
            await dbContext.SaveChangesAsync();

        }
    }
}
