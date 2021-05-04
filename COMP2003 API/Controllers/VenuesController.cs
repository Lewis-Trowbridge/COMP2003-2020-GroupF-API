using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2003_API.Models;
using COMP2003_API.Requests;
using COMP2003_API.Responses;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;


namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly COMP2003_FContext _context;

        public VenuesController(COMP2003_FContext context)
        {
            _context = context;
        }

        //api/venues/tablesAvailable?venueId=1&partySize=4&bookingTime=2001-12-25 18:10:00
        [HttpGet("tablesAvailable")]
        public async Task<ActionResult<List<VenueTablesAvailableResult>>> TablesAvailable(int venueId, int partySize, DateTime bookingTime)
        {
            List<VenueTablesAvailableResult> returnResults = new List<VenueTablesAvailableResult>();

            List<VenueTables> allPossibleTables = new List<VenueTables>();
            List<Bookings> allCurrentBookings = new List<Bookings>();

            allPossibleTables = await _context.VenueTables.Where(
            venueTable => (venueTable.VenueId == venueId && partySize <= venueTable.VenueTableCapacity)
            ).ToListAsync();

            allCurrentBookings = await _context.Bookings.Where(
            bookingTables => bookingTables.VenueId == venueId
            ).ToListAsync();

            foreach (VenueTables venueTableView in allPossibleTables)
            {
                VenueTablesAvailableResult newResult = new VenueTablesAvailableResult();
                newResult.TableId = venueTableView.VenueTableId;
                newResult.NumberOfSeats = venueTableView.VenueTableCapacity;
                newResult.VenueTableNumber = venueTableView.VenueTableNum;

                bool valid = true;
                foreach (Bookings bookingView in allCurrentBookings)
                {
                    if (bookingView.VenueTableId == venueTableView.VenueTableId)
                    {
                        if ((bookingView.BookingTime.AddHours(2) > bookingTime && bookingView.BookingTime.AddHours(-2) < bookingTime))
                        {
                            valid = false;
                        }
                    }                    

                }

                if (valid)
                {
                    returnResults.Add(newResult);
                }
                
                
            }

            return Ok(returnResults);
        }


        //api/venues/bookTable?venueTableId=2&customerId=3&bookingTime=2008-08-03 15:10:00&bookingSize=1
        //EXEC book_table @venue_table_id = 2, @customer_id = 3, @booking_time = '2001-12-25 18:10:00', @booking_size = 5
        [HttpPost("bookTable")]
        public async Task<ActionResult<CreationResult>> BookTable(CreateBooking createBooking)
        {
            int venueTableId = createBooking.VenueTableId;
            // Replace with retrieval of customer ID from HttpContext when authentication is implemented
            int customerId = createBooking.CustomerId;
            DateTime bookingTime = createBooking.BookingDateTime;
            int bookingSize = createBooking.BookingSize;

            int[] sc;             
            sc = 
                await CallBookTableSP(venueTableId, customerId, bookingTime, bookingSize);

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

        private async Task<int[]> CallBookTableSP(int venueTableId, int customerId, DateTime bookingTime, int bookingSize)
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

            await _context.Database.ExecuteSqlRawAsync("EXEC book_table @venue_table_id, @customer_id, @booking_time, @booking_size, @booking_id OUTPUT, @status_code OUTPUT", parameters);

            int[] output = new int[2];
            output[0] = Convert.ToInt32(parameters[4].Value);
            output[1] = Convert.ToInt32(parameters[5].Value);

            return output;
        }

        [HttpGet("view")]
        public async Task<ActionResult<AppVenueView>> View(int venueId)
        {
            try
            {
                AppVenueView venue = await _context.AppVenueView
                .Where(venue => venue.VenueId.Equals(venueId))
                .FirstAsync();
                return Ok(venue);
            }

            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet("top")]
        public async Task<ActionResult<List<MinifiedVenueResult>>> ViewTop()
        {
            // Specify variable in order for easy changing if necessary in future
            int numberToReturn = 30;
            List<MinifiedVenueResult> topViews = await _context.AppVenueView
                .Take(numberToReturn)
                .Select(view => new MinifiedVenueResult
                {
                    VenueId = view.VenueId,
                    VenueName = view.VenueName,
                    VenueCity = view.City,
                    VenuePostcode = view.VenuePostcode
                })
                .ToListAsync();
            return Ok(topViews);
        }


        //api/venues/search? searchString = seafood
        [HttpGet("search")]
        public async Task<ActionResult<List<MinifiedVenueResult>>> Search(string searchString)
        {
            List<MinifiedVenueResult> results = new List<MinifiedVenueResult>();
            List<AppVenueView> venuesSearched = new List<AppVenueView>();

            if (IsPostcode(searchString)) 
            {
                //Loop through, rate postcode, put in list.   

                venuesSearched.AddRange(_context.AppVenueView.AsEnumerable().Where(
                    venue => RatingPostcode(searchString, venue.VenuePostcode) == 4
                    ).ToList());

                venuesSearched.AddRange(_context.AppVenueView.AsEnumerable().Where(
                    venue => RatingPostcode(searchString, venue.VenuePostcode) == 3
                    ).ToList());

                venuesSearched.AddRange(_context.AppVenueView.AsEnumerable().Where(
                    venue => RatingPostcode(searchString, venue.VenuePostcode) == 2
                    ).ToList());

                venuesSearched.AddRange(_context.AppVenueView.AsEnumerable().Where(
                    venue => RatingPostcode(searchString, venue.VenuePostcode) == 1
                    ).ToList());


            }

            else
            {
                // Db lookup where name or city contains search string

                for (int i = 100; i>= 20; i--)
                {
                    venuesSearched.AddRange(_context.AppVenueView.AsEnumerable().Where(
                    venue => RatingLevenshteinDistance(searchString, venue.City) == i ^
                    RatingLevenshteinDistance(searchString, venue.VenueName) == i
                    ).ToList());
                }

            }

            // Extract the data we need from the venue views
            foreach (AppVenueView venueView in venuesSearched)
            {
                MinifiedVenueResult newResult = new MinifiedVenueResult();
                newResult.VenueId = venueView.VenueId;
                newResult.VenueName = venueView.VenueName;
                newResult.VenueCity = venueView.City;
                newResult.VenuePostcode = venueView.VenuePostcode;

                results.Add(newResult);
            }

            return Ok(results);
        }

        //https://stackoverflow.com/questions/9453731/how-to-calculate-distance-similarity-measure-of-given-2-strings
        private int RatingLevenshteinDistance(string inputWord, string compareAgainst)
        {            

            if (String.IsNullOrEmpty(inputWord) && String.IsNullOrEmpty(compareAgainst))
            {
                return 0;
            }
            if (String.IsNullOrEmpty(inputWord))
            {
                return 0;
            }
            if (String.IsNullOrEmpty(compareAgainst))
            {
                return 0;
            }

            inputWord = inputWord.ToUpper();
            compareAgainst = compareAgainst.ToUpper();

            int lengthInput = inputWord.Length;
            int lengthCompare = compareAgainst.Length;

            int[,] distance = new int[lengthInput + 1, lengthCompare + 1];

            for (int i = 0; i <= lengthInput; distance[i, 0] = i++) ;
            for (int j = 0; j <= lengthCompare; distance[0, j] = j++) ;

            for (int i = 1; i <= lengthInput; i++)
                for (int j = 1; j <= lengthCompare; j++)
                {
                    int cost = compareAgainst[j - 1] == inputWord[i - 1] ? 0 : 1;
                    distance[i, j] = Math.Min
                        (
                        Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                        distance[i - 1, j - 1] + cost
                        );
                }

            int editDifference = distance[lengthInput, lengthCompare];

            int longestWord = lengthInput;
            if (lengthCompare > longestWord)
            {
                longestWord = lengthCompare;
            }

            double percentageDifference = (100/Convert.ToDouble(longestWord)) * (longestWord - editDifference);

            return (Convert.ToInt32(percentageDifference));
        }
        private int RatingPostcode(string postcodeInputUser, string postcodeInputCompare)
        {
            //return rating based on the postcode input by the user and in stored restaurants //input postcode compared to in db, proximity rating
            //using ToUpper() and Replace(" ", String.Empty) so Regex can read it
            //postcodes dont always work for proximity - e.g. pl1 1et isnt necessarily closer to pl1 8bz than pl2 1et, ill keep in all of it, but when structuring output, bear it in mind.
            Regex rx = new Regex("^(?:(?<a>[Gg][Ii][Rr])(?<d>)(?<s>0)(?<u>[Aa]{2}))|(?:(?:(?:(?<a>[A-Za-z])(?<d>[0-9]{1,2}))|(?:(?:(?<a>[A-Za-z][A-Ha-hJ-Yj-y])(?<d>[0-9]{1,2}))|(?:(?:(?<a>[A-Za-z])(?<d>[0-9][A-Za-z]))|(?:(?<a>[A-Za-z][A-Ha-hJ-Yj-y])(?<d>[0-9]?[A-Za-z])))))(?<s>[0-9])(?<u>[A-Za-z]{2}))$", RegexOptions.None | RegexOptions.Compiled);
            Match matchUser = rx.Match(postcodeInputUser.ToUpper().Replace(" ", String.Empty));
            Match matchCompare = rx.Match(postcodeInputCompare.ToUpper().Replace(" ", String.Empty));
            int score = 0;

            if (matchCompare.Success)
            {
                if (matchUser.Groups["a"].Value == matchCompare.Groups["a"].Value)
                {
                    score += 1;
                    if (matchUser.Groups["d"].Value == matchCompare.Groups["d"].Value)
                    {
                        score += 1;
                        if (matchUser.Groups["s"].Value == matchCompare.Groups["s"].Value)
                        {
                            score += 1;
                            if (matchUser.Groups["u"].Value == matchCompare.Groups["u"].Value)
                            {
                                score += 1;
                            }
                        }
                    }
                }
            }
            return score;
        }


        private bool IsPostcode(string searchString)
        {
            if(searchString != null)
            {           
                Regex rx = new Regex(@"^([A-Z][A-HJ-Y]?\d[A-Zz\d]??\d[A-Z]{2}|GIR ?0A{2})$", RegexOptions.IgnoreCase | RegexOptions.Compiled); //^ $ start, end of text to prevent postcode in a sentence, might be worth changing later?
                Match match = rx.Match(searchString.Replace(" ", String.Empty)); //Replace " " with blank so that regex can read it, might be worth changing the regex later
                return match.Success;

            }
            else { return false; }
        }
    }
}
