﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using EventPlanner.Models;

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

        public IActionResult Privacy()
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        [HttpGet("event/new")]
        public IActionResult NewEvent()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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