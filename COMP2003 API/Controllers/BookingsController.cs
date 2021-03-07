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

        [HttpGet]
        public Task<ActionResult<AppBookingsView>> View(int bookingId)
        {
            throw new NotImplementedException();
        }
    }
}
