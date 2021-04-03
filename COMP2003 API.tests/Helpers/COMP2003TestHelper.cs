using System;
using Xunit;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    public class COMP2003TestHelper
    {
        public static COMP2003_FContext GetDbContext()
        {
            // Configuration should occur through
            COMP2003_FContext dbContext = new COMP2003_FContext();
            return dbContext;
        }

        public static Customers GetTestCustomer(int id)
        {
            Customers customer = new Customers();
            customer.CustomerId = id;
            customer.CustomerName = "Test User";
            customer.CustomerContactNumber = "01984454191";
            customer.CustomerUsername = "TestUser";
            customer.CustomerPassword = "TestPassword";

            return customer;
        }

        public static Venues GetTestVenue(int id)
        {
            Venues venue = new Venues
            {
                VenueId = id,
                VenueName = "Test Venue",
                AddLineOne = "Test line one",
                AddLineTwo = "Test line two",
                County = "Test county",
                City = "Test city",
                VenuePostcode = "PL4 8AA"
            };
            return venue;
        }

        public static VenueTables GetTestVenueTable(int id)
        {
            VenueTables venueTable = new VenueTables
            {
                VenueTableId = id,
                VenueTableNum = id + 1,
                VenueTableCapacity = 6
            };
            return venueTable;
        }

        public static Bookings GetTestBookings(int id)
        {
            Bookings booking = new Bookings
            {
                BookingId = id,
                BookingSize = 3,
                BookingTime = DateTime.Now,
                Venue = GetTestVenue(0),
                VenueTable = GetTestVenueTable(0)
            };
            return booking;
        }

        public static BookingAttendees GetTestBookingAttendee(Bookings booking)
        {
            BookingAttendees bookingAttendee = new BookingAttendees
            {
                Booking = booking,
                Customer = GetTestCustomer(0)
            };
            return bookingAttendee;
        }
    }
}
