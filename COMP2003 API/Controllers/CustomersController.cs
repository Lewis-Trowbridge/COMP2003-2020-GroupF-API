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
using BCrypt.Net;
using PhoneNumbers;

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
        public async Task<ActionResult<CreationResult>> Create(CreateCustomer customer)
        {

            CreationResult result = new CreationResult();
            if (!ModelState.IsValid)
            {
                result.Success = false;
                result.Message = "An unspecified validation error has occured.";
                return BadRequest(result);
            }
            // Check phone number is valid and return specific error message if it is not
            string formattedContactNumber = TryConvertContactNumber(customer.CustomerContactNumber);
            if (formattedContactNumber == null)
            {
                result.Success = false;
                result.Message = "A validation error has occured with the submitted contact number.";
                return BadRequest(result);
            }

            // Hash password using BCrypt using OWASP's recommended work factor of 12
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer.CustomerPassword, workFactor: 12);

            string response = await CallAddCustomerSP(customer.CustomerName, formattedContactNumber, customer.CustomerUsername, hashedPassword);
            switch (response.Substring(0, 3))
            {
                case "200":
                    int newId = Convert.ToInt32(response.Substring(3));
                    result.Success = true;
                    result.Id = newId;
                    result.Message = "A customer account has been created.";
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

        [HttpGet("view")]
        public async Task<ActionResult<MinifiedCustomerResult>> View(int customerId)
        {
            try
            {
                MinifiedCustomerResult result = await _context.Customers
                    .Where(customer => customer.CustomerId.Equals(customerId))
                    .Select(customer => new MinifiedCustomerResult
                    {
                        CustomerId = customer.CustomerId,
                        CustomerName = customer.CustomerName,
                        CustomerContactNumber = customer.CustomerContactNumber,
                        CustomerUsername = customer.CustomerUsername
                    })
                    .FirstAsync();
                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }


        }

        [HttpDelete("delete")]
        public async Task<ActionResult<DeletionResult>> Delete(int customerId)
        {
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

        private string TryConvertContactNumber(string contactNumber)
        {
            var phoneNumberUtil = PhoneNumberUtil.GetInstance();
            try
            {
                var phoneNumber = phoneNumberUtil.Parse(contactNumber, "GB");
                if (phoneNumberUtil.IsValidNumberForRegion(phoneNumber, "GB")) 
                {
                    // If the phone number if a valid UK number, return the formatted string
                    return phoneNumberUtil.Format(phoneNumber, PhoneNumberFormat.E164);
                }
                // If the phone number is not a valid UK number, signal this with a null
                else
                {
                    return null;
                }
            }
            // If there are any general issues with the formatting of the phone number, signal this with a null
            catch (NumberParseException)
            {
                return null;
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


        [HttpPut("edit")]
        public async Task<ActionResult<EditResult>> Edit(EditCustomer customer)
        {
            EditResult result = new EditResult();
            if (!ModelState.IsValid)
            {
                result.Success = false;
                result.Message = "A validation error occured.";               
                return BadRequest(result);
            }

            // Hash password using BCrypt using OWASP's recommended work factor of 12
            string hashedPassword = "";
            if (customer.CustomerPassword != "" && customer.CustomerPassword != null)
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(customer.CustomerPassword, workFactor: 12);
            }
            
            int response = await CallEditCustomerSP(customer.CustomerId, customer.CustomerName, customer.CustomerContactNumber, customer.CustomerUsername, hashedPassword);            
            switch (response)
            {
                case 200:
                    result.Success = true;
                    result.Message = "Customer details edited.";
                    return Ok(result);
                case 404:
                    result.Success = false;
                    result.Message = "Customer not found.";
                    return StatusCode(404, result);

                default:
                    result.Success = false;
                    result.Message = "An unspecified server error has occured.";
                    return StatusCode(500, result);
            }
        }
        private async Task<int> CallEditCustomerSP(int customerId, string customerName, string customerContactNumber, string customerUsername, string hashedPassword)
        {
            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@customer_id", customerId);
            parameters[1] = new SqlParameter("@customer_name", customerName);
            parameters[2] = new SqlParameter("@customer_contact_number", customerContactNumber);
            parameters[3] = new SqlParameter("@customer_username", customerUsername);
            parameters[4] = new SqlParameter("@customer_password", hashedPassword);
            parameters[5] = new SqlParameter("@response_message", 0);

            parameters[5].Direction = System.Data.ParameterDirection.Output;

            await _context.Database.ExecuteSqlRawAsync("EXEC edit_customer @customer_id, @customer_name, @customer_contact_number, @customer_username, @customer_password, @response_message OUTPUT", parameters);

            return Convert.ToInt32(parameters[5].Value);
        }

    }
}
