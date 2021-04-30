using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using COMP2003_API.Responses;
using COMP2003_API.Models;

namespace COMP2003_API.Tests.Helpers
{
    class VenuesControllerTestHelper
    {
        public static MinifiedVenueResult GetMinifiedVenueResult(Venues venue)
        {
            MinifiedVenueResult result = new MinifiedVenueResult
            {
                VenueId = venue.VenueId,
                VenueName = venue.VenueName,
                VenueCity = venue.City,
                VenuePostcode = venue.VenuePostcode
            };
            return result;
        }

        public async static Task<VenueTables> InsertVenueTableSet(COMP2003_FContext dbContext)
        {
            Venues venue = COMP2003TestHelper.GetTestVenue(0);
            await dbContext.AddAsync(venue);
            await dbContext.SaveChangesAsync();

            VenueTables table = COMP2003TestHelper.GetTestVenueTable(0);
            table.VenueId = venue.VenueId;

            await dbContext.AddAsync(table);
            await dbContext.SaveChangesAsync();

            return table;
        }

        public static VenueTablesAvailableResult GetVenueTablesAvailableResult(VenueTables table)
        {
            VenueTablesAvailableResult result = new VenueTablesAvailableResult
            {
                TableId = table.VenueTableId,
                VenueTableNumber = table.VenueTableNum,
                NumberOfSeats = table.VenueTableCapacity
            };
            return result;
        }
    }
}
