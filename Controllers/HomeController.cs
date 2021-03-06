﻿using System;
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
                return Redirect("/");
            }
            ViewBag.Events = _context.Events.Include(u => u.Creator).Include(g => g.Guests).ThenInclude(us => us.User).Where(t => t.Creator == LoggedUser()).OrderBy(time => time.ScheduledAt);
            ViewBag.JoinedEvent = _context.Links.Include(u => u.User).Include(g => g.Event).ThenInclude(i => i.Creator).Where(t => t.UserId == LoggedUser().UserId).OrderBy(time => time.Event.ScheduledAt);
            Console.WriteLine(ViewBag.JoinedEvent);
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
                return Redirect("/");
            }
            //get current user
            ViewBag.BestDates = BestDate();
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
                if (newEvent.ScheduledAt < DateTime.Now)
                {
                    ModelState.AddModelError("ScheduledAt", "Activity must be in the future");
                    return View("NewEvent");
                }
                TimeSpan duration = new TimeSpan(0, newEvent.Duration, 0, 0);
                newEvent.EndAt = newEvent.ScheduledAt.Add(duration);
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
                return Redirect("/");
            
            User CurrentUser = _context.Users.Include(u => u.Friends).FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("LoggedUser"));
            List<User> FriendsOfUser = new List<User>();

            foreach (var item in CurrentUser.Friends)
            {   
                FriendsOfUser.Add(_context.Users.FirstOrDefault(u => u.UserId == item.TargetId));
                Console.WriteLine("Added to list.");
            }
            ViewBag.Friends = FriendsOfUser;
            ViewBag.CheckLink = _context.Links.FirstOrDefault(u => u.UserId == LoggedUser().UserId && u.EventId == EventId);
            ViewBag.CurrentUser = CurrentUser;
            Event CurrentEvent = _context.Events.Include(u => u.Guests).ThenInclude(u => u.User).FirstOrDefault(e => e.EventId == EventId);
            List<User> UsersAtEvent = new List<User>();
            foreach(var guest in CurrentEvent.Guests){
                User userToAdd = _context.Users.FirstOrDefault(u => u.UserId == guest.UserId);
                UsersAtEvent.Add(userToAdd);
            }
            ViewBag.UsersAtEvent = UsersAtEvent;
            ViewBag.CurrentEvent = CurrentEvent;
            return View();
        }

        [HttpGet("invitation")]
        public IActionResult Invitation()
        {
            ViewBag.RequestInvites = _context.Invites.Include(u => u.User).Include(e => e.Event).Where( d => d.TargetId == LoggedUser().UserId);
            ViewBag.Invites = _context.RequestedInvites.Include(u => u.User).Include(e => e.Event).Where( d => d.Requester == LoggedUser().UserId);
            return View();
        }
        [HttpGet("event/join/{eventId}")]
        public IActionResult JoinEvent(int eventId){
            Link newLink = new Link(){UserId = LoggedUser().UserId, EventId = eventId};
            _context.Add(newLink);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [HttpGet("event/leave/{eventId}")]
        public IActionResult LeaveEvent(int eventId){
            Link linkToDelete = _context.Links.FirstOrDefault(u => u.UserId == LoggedUser().UserId && u.EventId == eventId);
            _context.Remove(linkToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");

        }
        [HttpGet("delete/{eventId}")]
        public IActionResult DeleteEvent(int EventId)
        {
            if(LoggedUser() != null)
            {
                //get the event to delete
                Event eventToDelete = _context.Events.FirstOrDefault(e => e.EventId == EventId);
                _context.Remove(eventToDelete);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            else
            {
                return Redirect("/");
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
        [HttpGet("/link/{linkId}/delete")]
        public IActionResult DeleteLink(int linkId)
        {
            Console.WriteLine("Delete link " + linkId);
            Link LinkToDel = _context.Links.FirstOrDefault(l => l.LinkId == linkId);
            _context.Remove(LinkToDel);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/");
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
            }               
            catch (Exception ex)
            {  
                Console.WriteLine(ex);
            }

        }
        public void CheckForReminders()
        {
            //this function will look up each reminder that is due to be sent
            List<Reminder> remindersToSend = _context.Reminders
                .Include(r => r.Event)
                .Where(r => r.TimeToSendReminder < DateTime.Now)
                .ToList();
                Console.WriteLine($"There are {remindersToSend.Count} to send");
            foreach (Reminder item in remindersToSend)
            {
                SendReminder(item);
                var RemToDel = _context.Reminders.First(r => r.ReminderId == item.ReminderId);
                _context.Remove(item);
                _context.SaveChanges();
            }
        }
    }
}