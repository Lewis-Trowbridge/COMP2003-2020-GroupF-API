﻿using System;
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
    public class CustomersController : ControllerBase
    {
        private readonly COMP2003_FContext _context;

        public CustomersController(COMP2003_FContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public ActionResult<CreationResult> Create(string customerName, string customerContactNumber, string customerUsername, string customerPassword)
        {
            throw new NotImplementedException();
        }

    }
}