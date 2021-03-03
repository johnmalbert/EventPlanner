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

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("registration")]
        public IActionResult Registration(User user){
            if(ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in user");
                    return View("Register");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", user.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Register");
        }

        [HttpPost("logging")]
        public IActionResult Logging(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                var userInDb = _context.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if (userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalide Email/Password");
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");

                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Login");
                }
                HttpContext.Session.SetInt32("LoggedUser", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Login");
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(LoggedUser()==null){
                return RedirectToAction("Index");
            }
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
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}