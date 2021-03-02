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
    public class TimesController : Controller{
        private readonly MyContext _context;

        public TimesController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("viewavailability")]
        public IActionResult ViewAvailability(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.Me = _context.Users.Include(t => t.FreeTimes).FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.LastMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month-1);
            ViewBag.Month = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            ViewBag.StartCal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return View();
        }
        [HttpPost("newTime")]
        public IActionResult NewTime(Time newTime){
            if(ModelState.IsValid){
                newTime.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
                _context.Add(newTime);
                _context.SaveChanges();
                return RedirectToAction("ViewAvailability");
            }
            return View("ViewAvailability");
        }
    }
}