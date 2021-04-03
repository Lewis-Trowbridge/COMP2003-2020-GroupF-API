using System;
using Xunit;
using COMP2003_API.Controllers;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using COMP2003_API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace COMP2003_API.Tests.Controllers
{
    public class BookingsControllerTests
    {
        [Fact]
        public async void Delete_ExistingBooking_DeletesSuccessfully()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new BookingsController(dbContext);
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var expectedDeletionResult = ResponseTestHelper.GetSuccessfulBookingDeletionResult();

            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);

            // Act
            var actionResult = await controller.Delete(testBooking.BookingId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realDeletionResult = (DeletionResult)okObjectResult.Value;


            // Assert
            Assert.DoesNotContain(testBooking, dbContext.Bookings);
            Assert.DoesNotContain(testAttendee, dbContext.BookingAttendees);
            Assert.Equal(expectedDeletionResult, realDeletionResult);
        }

        [Fact]
        public async void Delete_NonexistentBooking_Fails()
        {
            // Arrange
            var dbContext = COMP2003TestHelper.GetDbContext();
            var controller = new BookingsController(dbContext);
            var expectedDeletionResult = ResponseTestHelper.GetFailedBookingDeletionResult();

            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);

            // Act
            var actionResult = await controller.Delete(testBooking.BookingId + 1);
            var notFoundObjectResult = actionResult.Result as NotFoundObjectResult;
            var realDeletionResult = (DeletionResult)notFoundObjectResult.Value;


            // Assert
            Assert.Contains(testBooking, dbContext.Bookings);
            Assert.Contains(testAttendee, dbContext.BookingAttendees);
            Assert.Equal(expectedDeletionResult, realDeletionResult);

            // Cleanup
            dbContext.RemoveRange(new { testBooking.Venue, testBooking.VenueTable, testAttendee.Customer, testAttendee, testBooking });
            await dbContext.SaveChangesAsync();
        }
    }
}
