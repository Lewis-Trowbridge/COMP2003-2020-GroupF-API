using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using COMP2003_API.Models;
using COMP2003_API.Responses;
using COMP2003_API.Requests;

namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly COMP2003_FContext _context;

        public BookingsController(COMP2003_FContext context)
        {
            _context = context;
        }

        [HttpGet("view")]

        public async Task<ActionResult<AppBookingsView>> View(int bookingId, int customerId)
        {
            try
            {
                // Gets a booking view where passed in booking ID and determined customer ID
                // match the booking belonging to this customer
                AppBookingsView booking = await _context.AppBookingsView.Where(
                booking => booking.BookingId.Equals(bookingId)
                && booking.CustomerId.Equals(customerId)).FirstAsync();
                // Pass back view information - no response object is needed since this view contains
                // all of the information needed across all tables
                return Ok(booking);
            }

            // If no booking is found, then this exception will be thrown
            catch (InvalidOperationException)
            {
                // Since the request was correctly formatted, 204 no content is the most fitting response
                // when no booking is found
                return NoContent();
            }
        }

        [HttpDelete("delete")]
        public async Task<ActionResult<DeletionResult>> Delete(int bookingId)
        {
            DeletionResult result = new DeletionResult();
            string response = await CallDeleteBookingSP(bookingId);
            switch (response)
            {
                case "200":
                    result.Success = true;
                    result.Message = "This booking has been deleted.";
                    return Ok(result);

                case "404":
                    result.Success = false;
                    result.Message = "Deletion failed - booking does not exist.";
                    return NotFound(result);

                default:
                    result.Success = false;
                    result.Message = "An unspecified server error has occured.";
                    return StatusCode(500, result);
            }

        }

        private async Task<string> CallDeleteBookingSP(int bookingId)
        {
            SqlParameter[] parameters = new SqlParameter[2];
            parameters[0] = new SqlParameter("@booking_id", bookingId);
            parameters[1] = new SqlParameter
            {
                ParameterName = "@response",
                Direction = System.Data.ParameterDirection.Output,
                Size = 100
            };
            await _context.Database.ExecuteSqlRawAsync("EXEC delete_booking @booking_id, @response OUTPUT", parameters);

            return (string)parameters[1].Value;
        }

        [HttpPut("edit")]
        public async Task<ActionResult<DeletionResult>> Edit(EditBooking booking)
        {
            EditResult result = new EditResult();
            if (!ModelState.IsValid)
            {
                result.Success = false;
                result.Message = "A validation error occured.";
                return BadRequest(result);
            }

            if (!booking.BookingTime.HasValue)
            {
                booking.BookingTime = new DateTime(1753, 1, 1, 12, 0, 0);
            }

            int response = await CallEditBookingSP(booking.BookingId, booking.BookingTime.Value, booking.BookingSize, booking.VenueTableId);
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
                case 400:
                    result.Success = false;
                    result.Message = "Booking already at that time.";
                    return StatusCode(404, result);

                default:
                    result.Success = false;
                    result.Message = "An unspecified server error has occured.";
                    return StatusCode(500, result);
            }

        }
        private async Task<int> CallEditBookingSP(int bookingId, DateTime bookingTime, int bookingSize, int venueTableId)
        {
            SqlParameter[] parameters = new SqlParameter[5];
            parameters[0] = new SqlParameter("@booking_id", bookingId);
            parameters[1] = new SqlParameter("@booking_time", bookingTime);
            parameters[2] = new SqlParameter("@booking_size", bookingSize);
            parameters[3] = new SqlParameter("@venue_table_id", venueTableId);
            parameters[4] = new SqlParameter("@status_code", 0);

            parameters[4].Direction = System.Data.ParameterDirection.Output;

            await _context.Database.ExecuteSqlRawAsync("EXEC edit_booking @booking_id, @booking_time, @booking_size, @venue_table_id, @status_code OUTPUT", parameters);

            return Convert.ToInt32(parameters[4].Value);
        }
    }
}
