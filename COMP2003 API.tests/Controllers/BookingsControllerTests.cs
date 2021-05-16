using System;
using System.Collections.Generic;
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

        private COMP2003_FContext dbContext;
        private BookingsController controller;

        public BookingsControllerTests()
        {
            dbContext = COMP2003TestHelper.GetDbContext();
            controller = new BookingsController(dbContext);
        }

        [Fact]
        public async void ViewHistory_WithValidCustomerID_ReturnsOrderedInformation()
        {
            // Arrange 
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            // Change times to the past
            testBooking.BookingTime = DateTime.Now.AddDays(-1);
            Bookings testBooking2 = COMP2003TestHelper.GetTestBookings(0);
            testBooking2.BookingTime = DateTime.Now.AddHours(-2);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            BookingAttendees testAttendee2 = COMP2003TestHelper.GetTestBookingAttendee(testBooking2);
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);
            // Swap over values to link to the same results
            testAttendee2.Customer = testAttendee.Customer;
            testBooking2.Venue = testBooking.Venue;
            testBooking2.VenueTable = testBooking.VenueTable;
            await dbContext.Bookings.AddAsync(testBooking2);
            await dbContext.BookingAttendees.AddAsync(testAttendee2);
            await dbContext.SaveChangesAsync();

            // Set up expected results in chronological order to test that ordering is taking place
            var expectedResultsList = new List<MinifiedBookingResult>();
            expectedResultsList.Add(ResponseTestHelper.GetMinifiedBookingResult(testBooking2));
            expectedResultsList.Add(ResponseTestHelper.GetMinifiedBookingResult(testBooking));

            // Act
            var actionResult = await controller.ViewHistory(testAttendee.CustomerId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResultsList = (List<MinifiedBookingResult>)okObjectResult.Value;

            // Assert
            Assert.Equal(expectedResultsList[0].BookingId, realResultsList[0].BookingId);
            Assert.Equal(expectedResultsList[0].BookingSize, realResultsList[0].BookingSize);
            Assert.Equal(expectedResultsList[0].VenueName, realResultsList[0].VenueName);
            Assert.Equal(expectedResultsList[0].VenuePostcode, realResultsList[0].VenuePostcode);
            // Check in a one second range due to different resolution of milliseconds in SQL server and native Windows
            Assert.True(BookingsControllerTestHelper.DateTimesInOneSecondRange(expectedResultsList[0].BookingDateTime, realResultsList[0].BookingDateTime));

            Assert.Equal(expectedResultsList[1].BookingId, realResultsList[1].BookingId);
            Assert.Equal(expectedResultsList[1].BookingSize, realResultsList[1].BookingSize);
            Assert.Equal(expectedResultsList[1].VenueName, realResultsList[1].VenueName);
            Assert.Equal(expectedResultsList[1].VenuePostcode, realResultsList[1].VenuePostcode);
            // Check in a one second range due to different resolution of milliseconds in SQL server and native Windows
            Assert.True(BookingsControllerTestHelper.DateTimesInOneSecondRange(expectedResultsList[1].BookingDateTime, realResultsList[1].BookingDateTime));


        }

        [Fact]
        public async void ViewHistory_WithInvalidCustomerID_Fails()
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var expectedDeletionResult = ResponseTestHelper.GetSuccessfulBookingDeletionResult();

            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);

            var actionResult = await controller.ViewHistory(testAttendee.CustomerId + 1);

            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void ViewUpcoming_WithValidCustomerID_ReturnsOrderedInformation()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();

            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            // Change times to the past
            testBooking.BookingTime = DateTime.Now.AddDays(1);
            Bookings testBooking2 = COMP2003TestHelper.GetTestBookings(0);
            testBooking2.BookingTime = DateTime.Now.AddHours(2);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            BookingAttendees testAttendee2 = COMP2003TestHelper.GetTestBookingAttendee(testBooking2);
            // Insert booking set to ensure all background values are present
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);
            // Swap over values to link to the same results
            testAttendee2.Customer = testAttendee.Customer;
            testBooking2.Venue = testBooking.Venue;
            testBooking2.VenueTable = testBooking.VenueTable;
            await dbContext.Bookings.AddAsync(testBooking2);
            await dbContext.BookingAttendees.AddAsync(testAttendee2);
            await dbContext.SaveChangesAsync();

            // Set up expected results in chronological order to test that ordering is taking place
            var expectedResultsList = new List<MinifiedBookingResult>();
            expectedResultsList.Add(ResponseTestHelper.GetMinifiedBookingResult(testBooking2));
            expectedResultsList.Add(ResponseTestHelper.GetMinifiedBookingResult(testBooking));

            // Act
            var actionResult = await controller.ViewUpcoming(testAttendee.CustomerId);
            var okObjectResult = actionResult.Result as OkObjectResult;
            var realResultsList = (List<MinifiedBookingResult>)okObjectResult.Value;

            // Assert
            Assert.Equal(expectedResultsList[0].BookingId, realResultsList[0].BookingId);
            Assert.Equal(expectedResultsList[0].BookingSize, realResultsList[0].BookingSize);
            Assert.Equal(expectedResultsList[0].VenueName, realResultsList[0].VenueName);
            Assert.Equal(expectedResultsList[0].VenuePostcode, realResultsList[0].VenuePostcode);
            // Check in a one second range due to different resolution of milliseconds in SQL server and native Windows
            Assert.True(BookingsControllerTestHelper.DateTimesInOneSecondRange(expectedResultsList[0].BookingDateTime, realResultsList[0].BookingDateTime));

            Assert.Equal(expectedResultsList[1].BookingId, realResultsList[1].BookingId);
            Assert.Equal(expectedResultsList[1].BookingSize, realResultsList[1].BookingSize);
            Assert.Equal(expectedResultsList[1].VenueName, realResultsList[1].VenueName);
            Assert.Equal(expectedResultsList[1].VenuePostcode, realResultsList[1].VenuePostcode);
            // Check in a one second range due to different resolution of milliseconds in SQL server and native Windows
            Assert.True(BookingsControllerTestHelper.DateTimesInOneSecondRange(expectedResultsList[1].BookingDateTime, realResultsList[1].BookingDateTime));


        }

        [Fact]
        public async void ViewUpcoming_WithInvalidCustomerID_Fails()
        {
            // Arrange
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            var expectedDeletionResult = ResponseTestHelper.GetSuccessfulBookingDeletionResult();

            // Act
            Bookings testBooking = COMP2003TestHelper.GetTestBookings(0);
            BookingAttendees testAttendee = COMP2003TestHelper.GetTestBookingAttendee(testBooking);
            await BookingsControllerTestHelper.InsertBookingSet(dbContext, testBooking, testAttendee);

            var actionResult = await controller.ViewUpcoming(testAttendee.CustomerId + 1);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async void Delete_ExistingBooking_DeletesSuccessfully()
        {
            // Arrange
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
            dbContext.Remove(testBooking);
            dbContext.Remove(testBooking.VenueTable);
            dbContext.Remove(testBooking.Venue);
            dbContext.Remove(testAttendee);
            dbContext.Remove(testAttendee.Customer);
            await dbContext.SaveChangesAsync();
        }
    }
}
