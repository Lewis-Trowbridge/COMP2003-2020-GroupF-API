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

        [HttpGet]
        public ActionResult<List<VenuesSearchResult>> Search(string searchString)
        {
            throw new NotImplementedException();
        }

        private bool IsPostcode(string searchString)
        {
            return false;
        }
    }
}
