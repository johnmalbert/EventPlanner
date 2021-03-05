using System;
using System.Threading;
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
using System.Net.Mail;  
using System.Text;

namespace EventPlanner.Controllers
{
    public class BirthdayController : Controller
    {
        private readonly MyContext _context;

        public BirthdayController(MyContext context)
        {
            _context = context;
        }
        [HttpGet("birthdaycard/{eventId}")]
        public IActionResult BirthdayCard (int eventId)
        {
            ViewBag.Messages = _context.Messages.Where(m => m.EventId == eventId);
            ViewBag.CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            return View();
        }

        [HttpPost("birthdaycard/{eventId}/post")]
        public IActionResult Post(Message newMessage, int eventId)
        {
            if(ModelState.IsValid)
            {
                Console.WriteLine(newMessage.Note);
                Console.WriteLine(newMessage.Creator);
                _context.Add(newMessage);
                _context.SaveChanges();
                return RedirectToAction("BirthdayCard");
            }
            ViewBag.Messages = _context.Messages.Where(m => m.EventId == eventId);
            ViewBag.CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == eventId);
            return View("BirthdayCard");            
        }
    }
}