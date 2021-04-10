using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    public class BookingsControllerTestHelper
    {
        public async static Task InsertBookingSet(COMP2003_FContext dbContext, Bookings testBooking, BookingAttendees testAttendees)
        {
            await dbContext.Venues.AddAsync(testBooking.Venue);

            await dbContext.SaveChangesAsync();

            testBooking.VenueId = testBooking.Venue.VenueId;
            testBooking.VenueTable.VenueId = testBooking.Venue.VenueId;

            await dbContext.VenueTables.AddAsync(testBooking.VenueTable);

            await dbContext.SaveChangesAsync();

            testBooking.VenueTableId = testBooking.VenueTable.VenueTableId;
            await dbContext.Bookings.AddAsync(testBooking);

            await dbContext.SaveChangesAsync();

            await dbContext.Customers.AddAsync(testAttendees.Customer);

            await dbContext.SaveChangesAsync();

            testAttendees.CustomerId = testAttendees.Customer.CustomerId;
            testAttendees.BookingId = testBooking.BookingId;

            await dbContext.BookingAttendees.AddAsync(testAttendees);

            await dbContext.SaveChangesAsync();

        }

        public static bool DateTimesInOneSecondRange(DateTime datetime1, DateTime datetime2)
        {
            return datetime1 >= datetime2.AddSeconds(-0.5) && datetime1 < datetime2.AddSeconds(0.5);
        }
    }
}
