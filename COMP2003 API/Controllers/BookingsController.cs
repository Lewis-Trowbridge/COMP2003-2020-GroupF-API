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

        [HttpGet("history")]
        public async Task<ActionResult<List<MinifiedBookingResult>>> ViewHistory(int customerId)
        {
            bool customerExists = await _context.Customers.AnyAsync(customer => customer.CustomerId.Equals(customerId));
            if (customerExists)
            {
                var bookings = _context.AppBookingsView
                    .Where(booking => booking.CustomerId.Equals(customerId))
                    .Where(booking => booking.BookingTime < DateTime.Now)
                    .OrderByDescending(booking => booking.BookingTime);

                List<MinifiedBookingResult> resultList = MinifyBookingViews(bookings);

                return Ok(resultList);
            }

            else
            {
                return NotFound();
            }
        }

        [HttpGet("upcoming")]
        public async Task<ActionResult<List<MinifiedBookingResult>>> ViewUpcoming(int customerId)
        {
            bool customerExists = await _context.Customers.AnyAsync(customer => customer.CustomerId.Equals(customerId));
            if (customerExists)
            {
                var bookings = _context.AppBookingsView
                    .Where(booking => booking.CustomerId.Equals(customerId))
                    .Where(booking => booking.BookingTime > DateTime.Now)
                    .OrderBy(booking => booking.BookingTime);
                List<MinifiedBookingResult> resultList = MinifyBookingViews(bookings);

                return Ok(resultList);
            }

            else
            {
                return NotFound();
            }
        }

        private List<MinifiedBookingResult> MinifyBookingViews(IOrderedQueryable<AppBookingsView> bookingViews)
        {
            List<MinifiedBookingResult> minifiedList = new List<MinifiedBookingResult>();
            foreach (var bookingView in bookingViews)
            {
                MinifiedBookingResult minifiedBooking = new MinifiedBookingResult
                {
                    BookingId = bookingView.BookingId,
                    BookingDateTime = bookingView.BookingTime,
                    BookingSize = bookingView.BookingSize,
                    VenueName = bookingView.VenueName,
                    VenuePostcode = bookingView.VenuePostcode
                };
                minifiedList.Add(minifiedBooking);
            }

            return minifiedList;
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
    }
}
