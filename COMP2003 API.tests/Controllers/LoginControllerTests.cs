﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
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

            LoginResult testResult = LoginControllerTestHelper.GetSuccessfulLoginResult(testCustomer);

            // Act
            var actionResult = await controller.Login(testCustomer.CustomerUsername, originalPassword);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResult = (LoginResult)okObjectResult.Value;

            // Assert
            Assert.Equal(testResult, realResult);
        }
    }
}
