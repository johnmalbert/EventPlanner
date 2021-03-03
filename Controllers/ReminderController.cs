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
    public class ReminderController : Controller
    {
        private readonly MyContext _context;

        public ReminderController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("reminders")]
        public IActionResult SetReminder()
        {
            //get the current user
            User CurrentUser = LoggedUser();
            
            //get all the events for the current user
            ViewBag.Events = _context.Events.Where(e => e.Creator == CurrentUser);
            ViewBag.CurrentUser = CurrentUser;
            return View();
        }

        [HttpGet("reminder/{num}")]
        public IActionResult SetIndividualReminder(int num)
        {
            //get the current user
            User CurrentUser = LoggedUser();
            //get the event 
            Event CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == num);

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentEvent = CurrentEvent;
            //return view
            return View();
        }

        public User LoggedUser() // this will return null if the user isn't logged in.
        {
            int? UserId = HttpContext.Session.GetInt32("LoggedUser");

            if (UserId == null) //user isn't logged in.
                return null;

            User CurrentUser = _context.Users.First(u => u.UserId == UserId);
            return CurrentUser;
        }

        //reminder message function
        [HttpPost("reminder/{eventNum}/create")]
        public IActionResult SetReminder(int eventNum, int timeBefore) //amount of time before the event to send reminder
        {
            // get current user
            User CurrentUser = LoggedUser();
            // get the event for the reminder
            Event CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == eventNum);
            //get the time before event to set the reminder
            TimeSpan timeBeforeSpan = new TimeSpan(0,timeBefore,0,0,0);
            DateTime setReminder = CurrentEvent.ScheduledAt - timeBeforeSpan;
            //create a new reminder
            Reminder newReminder = new Reminder{
                User = CurrentUser,
                Event = CurrentEvent,
                MessageBody = $"Hello {CurrentUser.FirstName}, \nYou your event {CurrentEvent.Title} is coming up!",
                MesssageSubject = $"Event Reminder - {CurrentEvent.Title}",
                to = CurrentUser.Email,
                TimeToSendReminder = setReminder
            };
            Console.WriteLine($"Reminder will be sent to {newReminder.to}, set for {newReminder.TimeToSendReminder}");

            return Redirect($"/reminder/{CurrentEvent.EventId}");
        }
    }
}
//jqzeventreminders@gmail.com
//JQZEvents is the p-w