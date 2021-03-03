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

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(LoggedUser()==null){
                return RedirectToAction("Index");
            }
            BestDate();
            ViewBag.Events = _context.Events.Include(u => u.Creator).Include(g => g.Guests).OrderBy(time => time.ScheduledAt);
            ViewBag.me = LoggedUser();
            ViewBag.Me = _context.Users.Include(t => t.FreeTimes).FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.LastMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month-1);
            ViewBag.Month = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            ViewBag.StartCal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return View();
        }

        [HttpGet("event/new")]
        public IActionResult NewEvent()
        {
            if(LoggedUser() == null)
            {
                return RedirectToAction("Index");
            }
            //get current user
            ViewBag.CurrentUser = LoggedUser();
            return View();
        }

        [HttpPost("event/create")]
        public IActionResult CreateEvent(Event newEvent)
        {
            Console.WriteLine("Created a new event");
            User CurrentUser = LoggedUser();
            if(ModelState.IsValid)
            {
                //add the event to the database
                Console.WriteLine($"Created event {newEvent.Title}");
                newEvent.Creator = CurrentUser;
                _context.Add(newEvent);
                _context.SaveChanges();
                return RedirectToAction("DisplayEvent", new {EventId = newEvent.EventId});
            }
            else
            {
                //return the view to correct errors
                Console.WriteLine("Not valid.");
                return View("NewEvent");
            }
            // route to the new event ID 
            // return RedirectToAction("Dashboard");
        }

        [HttpGet("event/{EventId}")]
        public IActionResult DisplayEvent(int EventId)
        {
            //see if user is logged in
            ViewBag.CurrentUser = LoggedUser();
            if(ViewBag.CurrentUser == null) //send back to index if not logged in.
                return RedirectToAction("Index");
            
            ViewBag.CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == EventId);
            return View();
        }

        [HttpGet("invitation/{EventId}")]
        public IActionResult Invitation(int EventId)
        {
            return View();
        }
        [HttpGet("delete/{eventId}")]
        public IActionResult DeleteEvent(int EventId)
        {
            if(LoggedUser() != null)
            {
                //get the event to delete
                Event eventToDelete = _context.Events.FirstOrDefault(e => e.EventId == EventId);
                var RemindersToDelete = _context.Reminders.Where(r => r.Event == eventToDelete);
                _context.Remove(eventToDelete);
                _context.Remove(RemindersToDelete);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public User LoggedUser() // this will return null if the user isn't logged in.
        {
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");

            if (UserId == null) //user isn't logged in.
                return null;

            User CurrentUser = _context.Users.First(u => u.UserId == UserId);
            return CurrentUser;
        }
        public Dictionary<DateTime,int> BestDate() // this will return null if the user isn't logged in.
        {
            List<DateTime> GoodTimes = new List<DateTime>();
                foreach( Friend fr in _context.Friends.Include(u =>u.User).ThenInclude(g => g.FreeTimes).Where(t => t.TargetId == LoggedUser().UserId && t.Status ==2)){
                    foreach(Time gt in fr.User.FreeTimes){
                            GoodTimes.Add(gt.StartAt);
                    }
                }
            var q = GoodTimes.GroupBy(x => x).Select(g => new {Value = g.Key, Count = g.Count()}).OrderByDescending(x => x.Count);
            Dictionary<DateTime,int> BestTimes = new Dictionary<DateTime,int>();
            foreach(var x in q){
                BestTimes.Add(x.Value,x.Count);
            }
            foreach(var l in BestTimes){
                Console.WriteLine(l.Key);
                Console.WriteLine(l.Value);
            }
            return BestTimes;
        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}