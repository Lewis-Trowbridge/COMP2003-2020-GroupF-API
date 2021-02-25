using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using COMP2003_API.Models;
using COMP2003_API.Responses;

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

        [HttpGet("search")]
        public ActionResult<List<VenuesSearchResult>> Search(string searchString)
        {
            if (IsPostcode(searchString))
            {
                throw new NotImplementedException();
            }
            else
            {
                List<AppVenueView> venuesSearched = _context.AppVenueView.Where(
                    venue => venue.VenueName.Contains(searchString) ||
                    venue.City.Contains(searchString)
                    ).ToList();
                List<VenuesSearchResult> results = new List<VenuesSearchResult>();
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
        }

        private bool IsPostcode(string searchString)
        {
            return false;
        }
    }
}
