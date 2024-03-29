﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using BCrypt.Net;

namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {

        private readonly COMP2003_FContext _context;

        public LoginController(COMP2003_FContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
        {
            LoginResult result;

            try
            {
                Customers storedCustomer = await _context.Customers
                    .Where(customer => customer.CustomerUsername.Equals(request.CustomerUsername))
                    .SingleAsync();

                if (BCrypt.Net.BCrypt.Verify(request.CustomerPassword, storedCustomer.CustomerPassword))
                {
                    result = new LoginResult
                    {
                        Success = true,
                        Id = storedCustomer.CustomerId,
                        Message = "Login successful."
                    };
                    return Ok(result);
                }

                else
                {
                    result = new LoginResult
                    {
                        Success = false,
                        Message = "Login failed."
                    };
                    return Unauthorized(result);
                }
            }
            catch (InvalidOperationException)
            {
                result = new LoginResult
                {
                    Success = false,
                    Message = "Login failed."
                };
                return Unauthorized(result);
            }
        }


    }
}
