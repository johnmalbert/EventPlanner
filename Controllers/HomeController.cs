using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventPlanner.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace EventPlanner.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet("event/new")]
        public IActionResult NewEvent()
        {
            return View();
        }

        [HttpGet("event/{EventId}")]
        public IActionResult DisplayEvent(int EventId)
        {
            return View();
        }

        [HttpGet("invitation/{EventId}")]
        public IActionResult Invitation(int EventId)
        {
            return View();
        }
    }
}