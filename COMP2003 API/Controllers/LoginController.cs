using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COMP2003_API.Models;
using COMP2003_API.Responses;

namespace COMP2003_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {

        private readonly COMP2003_FContext _context;

        public LoginController(COMP2003_FContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<LoginResult> Login(string customerUsername, string customerPassword)
        {
            throw new NotImplementedException();
        }


    }
}
