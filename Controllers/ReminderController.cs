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
using System.IO;  
using System.Net;  
using System.Net.Mail;  
using System.Text;

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
            if(CurrentUser == null)
                return RedirectToAction("Index");
            
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
            if(CurrentUser == null)
                return RedirectToAction("Index");

            //get the event 
            Event CurrentEvent = _context.Events.FirstOrDefault(e => e.EventId == num);

            ViewBag.CurrentUser = CurrentUser;
            ViewBag.CurrentEvent = CurrentEvent;
            ViewBag.Reminders = _context.Reminders
                .Include(r => r.User)
                .Include(r => r.Event)
                .Where(r => r.User == CurrentUser && r.Event == CurrentEvent);
            //return view
            return View();
        }

        [HttpGet("reminder/{reminderId}/delete")]
        public IActionResult DeleteReminder(int reminderId)
        {
            //get the reminder by number
            Reminder reminderToDelete = _context.Reminders.FirstOrDefault(r => r.ReminderId == reminderId);
            int EventId = reminderToDelete.EventId;
            _context.Remove(reminderToDelete);
            _context.SaveChanges();
            Console.WriteLine("Deleted a Reminder.");
            return Redirect($"/reminder/{EventId}");
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
            _context.Add(newReminder);
            Console.WriteLine($"From: {newReminder.from} pw {newReminder.PW}");
            SendReminder(newReminder);
            _context.SaveChanges();

            Console.WriteLine($"Reminder will be sent to {newReminder.to}, set for {newReminder.TimeToSendReminder}");

            return Redirect($"/reminder/{CurrentEvent.EventId}");
        }
        public void SendReminder(Reminder reminder)
        {
            MailMessage message = new MailMessage(reminder.from, reminder.to);
            message.Subject = reminder.MesssageSubject;
            message.Body = reminder.MessageBody;
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential(reminder.from, reminder.PW);
            client.EnableSsl = true;  
            client.UseDefaultCredentials = false;  
            client.Credentials = basicCredential1;  
            try   
            {  
                client.Send(message);
                Reminder reminderToDelete = _context.Reminders.FirstOrDefault(r => r == reminder);
                _context.Remove(reminderToDelete);
                _context.SaveChanges();
            }   
            
            catch (Exception ex)
            {  
                Console.WriteLine("Reminder not sent!");
                throw ex;  
            }
        }
    }
}
//jqzeventreminders@gmail.com
//JQZEvents is the p-w