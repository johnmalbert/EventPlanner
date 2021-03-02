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
        [HttpGet("registration")]
        public IActionResult Registration(){
            return View();
        }

        // Register is what processess the register form
        // if registration is valid saves the UserId under LoggedUser
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in user");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", user.UserId);
                return RedirectToAction("Dashboard");
            }
            return View("Index");
        }

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