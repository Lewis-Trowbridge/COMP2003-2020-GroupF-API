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
        public async void Create_ReturnsBadResult_ForNullParameters()
        {
            COMP2003_FContext dbContext = COMP2003TestHelper.GetDbContext();
            CustomersController controller = new CustomersController(dbContext);
            controller.ModelState.AddModelError("error", "Invalid Model parameters");

            ActionResult<CreationResult> result = await controller.Create(new Customers());

            Assert.IsType<BadRequestObjectResult>(result.Result);

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

        [Fact]
        public async void Edit_WithValidInputs_EditsSuccessfully()
        {

            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new CustomersController(dbContext);
            var testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            var expectedEditResult = ResponseTestHelper.GetSuccessfulAcccountEditResult();
            await dbContext.Customers.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            EditCustomer request = new EditCustomer();
            request.CustomerId = testCustomer.CustomerId;
            request.CustomerContactNumber = testCustomer.CustomerContactNumber + "edit";
            request.CustomerName = testCustomer.CustomerName + "edit";
            request.CustomerPassword = testCustomer.CustomerPassword;
            request.CustomerUsername = testCustomer.CustomerUsername + "edit";            

            // Act
            var actionResult = await controller.Edit(request);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realEditResult = (EditResult)okObjectResult.Value;

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedEditResult.Message, realEditResult.Message);
            Assert.Equal(expectedEditResult.Success, realEditResult.Success);

            // Cleanup
            dbContext.Customers.Remove(testCustomer);
            await dbContext.SaveChangesAsync();
        }

        [Fact]
        public async void Edit_WithValidNullInput_EditsSucessfully()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new CustomersController(dbContext);
            var testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            var expectedEditResult = ResponseTestHelper.GetSuccessfulAcccountEditResult();
            await dbContext.Customers.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            EditCustomer request = new EditCustomer();
            request.CustomerId = testCustomer.CustomerId;
            request.CustomerContactNumber = "";
            request.CustomerName = testCustomer.CustomerName + "editNull";
            request.CustomerPassword = "";
            request.CustomerUsername = "";

            // Act
            var actionResult = await controller.Edit(request);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realEditResult = (EditResult)okObjectResult.Value;

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedEditResult.Message, realEditResult.Message);
            Assert.Equal(expectedEditResult.Success, realEditResult.Success);

            // Cleanup
            dbContext.Customers.Remove(testCustomer);
            await dbContext.SaveChangesAsync();

        }

        [Fact]
        public async void Edit_WithPassword_EditsSuccessfully()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new CustomersController(dbContext);
            var testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            var expectedEditResult = ResponseTestHelper.GetSuccessfulAcccountEditResult();
            await dbContext.Customers.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            EditCustomer request = new EditCustomer();
            request.CustomerId = testCustomer.CustomerId;
            request.CustomerContactNumber = testCustomer.CustomerContactNumber + "edit";
            request.CustomerName = testCustomer.CustomerName + "edit";
            request.CustomerPassword = testCustomer.CustomerPassword + "edit";
            request.CustomerUsername = testCustomer.CustomerUsername + "edit";

            // Act
            var actionResult = await controller.Edit(request);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realEditResult = (EditResult)okObjectResult.Value;

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(expectedEditResult.Message, realEditResult.Message);
            Assert.Equal(expectedEditResult.Success, realEditResult.Success);

            // Cleanup
            dbContext.Customers.Remove(testCustomer);
            await dbContext.SaveChangesAsync();
        }

    }
}
