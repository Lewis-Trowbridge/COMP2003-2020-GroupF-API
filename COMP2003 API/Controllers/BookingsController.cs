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

        [HttpGet("history")]
        public async Task<ActionResult<List<MinifiedBookingResult>>> ViewHistory(int customerId)
        {
            bool customerExists = await _context.Customers.AnyAsync(customer => customer.CustomerId.Equals(customerId));
            if (customerExists)
            {
                var bookings = await _context.AppBookingsView
                    .Where(booking => booking.CustomerId.Equals(customerId))
                    .Where(booking => booking.BookingTime < DateTime.Now)
                    .Select(booking => new MinifiedBookingResult
                    {
                        BookingId = booking.BookingId,
                        BookingDateTime = booking.BookingTime,
                        BookingSize = booking.BookingSize,
                        VenueName = booking.VenueName,
                        VenuePostcode = booking.VenuePostcode
                    })
                    .OrderByDescending(booking => booking.BookingDateTime)
                    .ToListAsync();

                return Ok(bookings);
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
                var bookings = await _context.AppBookingsView
                    .Where(booking => booking.CustomerId.Equals(customerId))
                    .Where(booking => booking.BookingTime > DateTime.Now)
                    .Select(booking => new MinifiedBookingResult
                    {
                        BookingId = booking.BookingId,
                        BookingDateTime = booking.BookingTime,
                        BookingSize = booking.BookingSize,
                        VenueName = booking.VenueName,
                        VenuePostcode = booking.VenuePostcode
                    })
                    .OrderBy(booking => booking.BookingDateTime)
                    .ToListAsync();
                
                return Ok(bookings);
            }

            else
            {
                return NotFound();
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
        public async Task<ActionResult<DeletionResult>> Edit(EditBookingRequest booking)
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
                booking.BookingTime = new DateTime(1753, 1, 1, 12, 0, 0); //This is the minimum dateTimethat can be passed through
            }

            int response = await CallEditBookingSP(booking.BookingId, booking.BookingTime.Value, booking.BookingSize, booking.VenueTableId);
            switch (response)
            {
                case 200:
                    result.Success = true;
                    result.Message = "Booking details edited.";
                    return Ok(result);
                case 404:
                    result.Success = false;
                    result.Message = "Booking not found.";
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



        //api/bookings/bookTable?venueTableId=2&customerId=3&bookingTime=2008-08-03 15:10:00&bookingSize=1
        //EXEC book_table @venue_table_id = 2, @customer_id = 3, @booking_time = '2001-12-25 18:10:00', @booking_size = 5
        [HttpPost("create")]
        public async Task<ActionResult<CreationResult>> Create(CreateBooking createBooking)
        {
            int venueTableId = createBooking.VenueTableId;
            // Replace with retrieval of customer ID from HttpContext when authentication is implemented
            int customerId = createBooking.CustomerId;
            DateTime bookingTime = createBooking.BookingDateTime;
            int bookingSize = createBooking.BookingSize;

            int[] sc;
            sc =
                await CallCreateBookingSP(venueTableId, customerId, bookingTime, bookingSize);

            CreationResult result = new CreationResult();

            result.Message = "Booking unsuccessful";
            result.Success = false;
            if (sc[1] == 201)
            {
                result.Success = true;
                result.Id = sc[0];
                result.Message = "Booking created";
                return Ok(result);
            }

            return StatusCode(sc[1]);
        }
        private async Task<int[]> CallCreateBookingSP(int venueTableId, int customerId, DateTime bookingTime, int bookingSize)
        {

            SqlParameter[] parameters = new SqlParameter[6];
            parameters[0] = new SqlParameter("@venue_table_id", venueTableId);
            parameters[1] = new SqlParameter("@customer_id", customerId);
            parameters[2] = new SqlParameter("@booking_time", bookingTime.ToString("yyyy-MM-dd HH:mm:ss"));
            parameters[3] = new SqlParameter("@booking_size", bookingSize);
            parameters[4] = new SqlParameter("@booking_id", 0);
            parameters[5] = new SqlParameter("@status_code", 0);

            parameters[4].Direction = System.Data.ParameterDirection.Output;
            parameters[5].Direction = System.Data.ParameterDirection.Output;

            await _context.Database.ExecuteSqlRawAsync("EXEC create_booking @venue_table_id, @customer_id, @booking_time, @booking_size, @booking_id OUTPUT, @status_code OUTPUT", parameters);

            int[] output = new int[2];
            output[0] = Convert.ToInt32(parameters[4].Value);
            output[1] = Convert.ToInt32(parameters[5].Value);

            return output;
        }
    }
}
