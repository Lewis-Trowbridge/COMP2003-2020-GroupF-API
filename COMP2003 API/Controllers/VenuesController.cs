using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2003_API.Models;
using COMP2003_API.Responses;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;


namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenuesController : ControllerBase
    {
        private readonly cleanTableDbContext _context;

        public VenuesController(cleanTableDbContext context)
        {
            _context = context;
        }

        //api/venues/bookTable?venueTableId=2&customerId=3&bookingTime=2008-08-03 15:10:00&bookingSize=1
        //EXEC book_table @venue_table_id = 2, @customer_id = 3, @booking_time = '2001-12-25 18:10:00', @booking_size = 5
        [HttpPost("bookTable")]
        public async Task<ActionResult> BookTable(int venueTableId, int customerId, DateTime bookingTime, int bookingSize)
        {
            int sc;                    
            sc = await CallBookTableSP(venueTableId, customerId, bookingTime, bookingSize);

            return StatusCode(sc);
        }

        private async Task<int> CallBookTableSP(int venueTableId, int customerId, DateTime bookingTime, int bookingSize)
        {
            int rowsAffected = await _context.Database.ExecuteSqlRawAsync("EXEC book_table @venue_table_id = "
                + venueTableId.ToString() + ", @customer_id = " + customerId.ToString() + ", @booking_time = '" + bookingTime.ToString("yyyy-MM-dd HH:mm:ss") + "', @booking_size = " +
                bookingSize.ToString());

            if (rowsAffected == 2)
            {
                return 200;
            }

            return 500;
        }


        [HttpGet("search")]
        public ActionResult<List<VenuesSearchResult>> Search(string searchString)
        {
            List<VenuesSearchResult> results = new List<VenuesSearchResult>();
            List<AppVenueView> venuesSearched = new List<AppVenueView>();

            if (IsPostcode(searchString)) 
            {
                //Loop through, rate postcode, put in list.
                venuesSearched = _context.AppVenueView.AsEnumerable().Where(
                    venue => RatingPostcode(searchString, venue.VenuePostcode) >= 1
                    ).ToList();
            }

            else
            {
                // Db lookup where name or city contains search string
                venuesSearched = _context.AppVenueView.Where(
                    venue => venue.VenueName.Contains(searchString) ||
                    venue.City.Contains(searchString)
                    ).ToList();
            }

            // Extract the data we need from the venue views
            foreach (AppVenueView venueView in venuesSearched)
            {
                VenuesSearchResult newResult = new VenuesSearchResult();
                newResult.Id = venueView.VenueId;
                newResult.Name = venueView.VenueName;
                newResult.City = venueView.City;
                newResult.Postcode = venueView.VenuePostcode;

                results.Add(newResult);
            }

            return results;
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
            Regex rx = new Regex(@"^([A-Z][A-HJ-Y]?\d[A-Zz\d]??\d[A-Z]{2}|GIR ?0A{2})$", RegexOptions.IgnoreCase | RegexOptions.Compiled); //^ $ start, end of text to prevent postcode in a sentence, might be worth changing later?
            Match match = rx.Match(searchString.Replace(" ", String.Empty)); //Replace " " with blank so that regex can read it, might be worth changing the regex later
            return match.Success;
        }
    }
}
