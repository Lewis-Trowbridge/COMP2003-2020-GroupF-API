using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using BCrypt;

namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly COMP2003_FContext _context;

        public CustomersController(COMP2003_FContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<ActionResult<CreationResult>> Create(Customers customer)
        {
            CreationResult result = new CreationResult();
            if (!ModelState.IsValid)
            {
                result.Success = false;
                result.Message = "A validation error occured.";
                return BadRequest(result);
            }

            // Hash password using BCrypt using OWASP's recommended work factor of 12
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer.CustomerPassword, workFactor: 12);
            string response = await CallAddCustomerSP(customer.CustomerName, customer.CustomerContactNumber, customer.CustomerUsername, hashedPassword);
            switch (response.Substring(0, 3))
            {
                case "200":
                    int newId = Convert.ToInt32(response.Substring(3));
                    result.Success = true;
                    result.Id = newId;
                    result.Message = "Customer account created.";
                    return Ok(result);

                case "208":
                    result.Success = false;
                    result.Message = "An account with that username already exists.";
                    return StatusCode(208, result);

                default:
                    result.Success = false;
                    result.Message = "An unspecified server error has occured.";
                    return StatusCode(500, result);
            }

        }

        [HttpDelete("delete")]
        public async Task<ActionResult<DeletionResult>> Delete(DeletionRequest deletionRequest)
        {
            int customerId = deletionRequest.Id;
            DeletionResult result = new DeletionResult();
            string response = await CallDeleteCustomerSP(customerId);

            switch (response)
            {
                case "200":
                    result.Success = true;
                    result.Message = "This account has been deleted.";
                    return Ok(result);

                case "404":
                    result.Success = false;
                    result.Message = "Deletion failed - account does not exist.";
                    return NotFound(result);

                default:
                    result.Success = false;
                    result.Message = "An unspecifed server error has occured.";
                    return StatusCode(500, result);
            }
        }

        private async Task<string> CallAddCustomerSP(string customerName, string customerContactNumber, string customerUsername, string hashedPassword)
        {
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@customer_name", customerName);
            parameters[1] = new SqlParameter("@customer_contact_number", customerContactNumber);
            parameters[2] = new SqlParameter("@customer_username", customerUsername);
            parameters[3] = new SqlParameter("@customer_password", hashedPassword);
            parameters[4] = new SqlParameter
            {
                ParameterName = "@response",
                Direction = System.Data.ParameterDirection.Output,
                Size = 100
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC add_customer @customer_name, @customer_contact_number, @customer_username, @customer_password, @response OUTPUT", parameters);

            return (string)parameters[4].Value;
        }

        private async Task<string> CallDeleteCustomerSP(int customerId)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@customer_id", customerId);
            parameters[1] = new SqlParameter
            {
                ParameterName = "@response",
                Direction = System.Data.ParameterDirection.Output,
                Size = 100
            };

            await _context.Database.ExecuteSqlRawAsync("EXEC delete_customer @customer_id, @response OUTPUT", parameters);

            return (string)parameters[1].Value;
        }

    }
}
