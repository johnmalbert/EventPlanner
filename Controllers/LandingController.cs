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
    public class LandingController : Controller
    {
        private readonly MyContext _context;

        public LandingController(MyContext context)
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
                    ModelState.AddModelError("Email", "Email is already in use");
                    return View("Register");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                _context.Add(user);
                _context.SaveChanges();
                HttpContext.Session.SetInt32("LoggedUser", user.UserId);
                return Redirect("/Dashboard");
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
                    ModelState.AddModelError("Email", "Invalid Email/Password");
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
                return Redirect("Dashboard");
            }
            return View("Login");
        }
    }
}