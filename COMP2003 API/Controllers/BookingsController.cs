using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2003_API.Models;

namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly cleanTableDbContext _context;

        public BookingsController(cleanTableDbContext context)
        {
            _context = context;
        }

        [HttpGet("view")]
        public async Task<ActionResult<AppBookingsView>> View(int bookingId)
        {
            // This is where the access of the customer ID would go
            // which would likely use HttpContext to obtain the customer ID - it is hardcoded for now
            int customerId = 1;
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
    }
}
