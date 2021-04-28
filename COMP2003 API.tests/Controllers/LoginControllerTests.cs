using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using COMP2003_API.Tests.Helpers;
using Xunit;
using BCrypt.Net;

namespace COMP2003_API.Tests.Controllers
{
    public class LoginControllerTests
    {
        private LoginController controller;
        private COMP2003_FContext dbContext;

        public LoginControllerTests()
        {
            dbContext = new COMP2003_FContext();
            controller = new LoginController(dbContext);
        }

        [Fact]
        public async void Login_WithValidInputs_LogsInSuccessfully()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            Customers testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            string originalPassword = testCustomer.CustomerPassword;
            testCustomer.CustomerPassword = BCrypt.Net.BCrypt.HashPassword(originalPassword, workFactor: 12);
            await dbContext.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            LoginRequest testRequest = new LoginRequest
            {
                CustomerUsername = testCustomer.CustomerUsername,
                CustomerPassword = originalPassword
            };
            LoginResult testResult = LoginControllerTestHelper.GetSuccessfulLoginResult(testCustomer);

            // Act
            var actionResult = await controller.Login(testRequest);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (LoginResult)okObjectResult.Value;

            // Assert
            Assert.Equal(testResult, realResult);
        }

        [Theory]
        [InlineData("TestUser", "Wrong password")]
        [InlineData("Wrong user", "TestPassword")]
        [InlineData("Wrong user", "Wrong password")]
        public async void Login_WithInvalidInputs_Fails(string username, string password)
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            Customers testCustomer = COMP2003TestHelper.GetTestCustomer(0);
            string originalPassword = testCustomer.CustomerPassword;
            testCustomer.CustomerPassword = BCrypt.Net.BCrypt.HashPassword(originalPassword, workFactor: 12);
            await dbContext.AddAsync(testCustomer);
            await dbContext.SaveChangesAsync();

            LoginRequest testRequest = new LoginRequest
            {
                CustomerUsername = username,
                CustomerPassword = password
            };
            LoginResult testResult = LoginControllerTestHelper.GetFailedLoginResult();

            // Act
            var actionResult = await controller.Login(testRequest);
            var unauthorisedResult = actionResult.Result as UnauthorizedObjectResult;
            var realResult = (LoginResult)unauthorisedResult.Value;

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(actionResult.Result);
            Assert.Equal(testResult, realResult);

        }
    }
}
