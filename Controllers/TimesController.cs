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
using System.Net.Mail;  
using System.Text;

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
            ViewBag.BestDates = BestDate();
            ViewBag.Me = _context.Users.Include(t => t.FreeTimes).FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser"));
            ViewBag.LastMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month-1);
            ViewBag.Month = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            ViewBag.StartCal = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            return View();
        }
        [HttpPost("newTime")]
        public IActionResult NewTime(Time newTime){
            if(ModelState.IsValid){
                if(_context.Times.Any(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser") && u.StartAt == newTime.StartAt)){
                    return RedirectToAction("ViewAvailability");
                }
                newTime.UserId = (int)HttpContext.Session.GetInt32("LoggedUser");
                _context.Add(newTime);
                _context.SaveChanges();
                return RedirectToAction("ViewAvailability");
            }
            return View("ViewAvailability");
        }
        [HttpGet("viewFriends")]
        public IActionResult ViewFriends(){
            if(HttpContext.Session.GetInt32("LoggedUser") == null)
            {
                return Redirect("/");
            }
            ViewBag.Friends = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 2);
            ViewBag.ISent = _context.Friends.Include(u => u.User).Where(t => t.UserId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
            ViewBag.Invites = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
            return View();
        }
        [HttpPost("findFriend")]
        public IActionResult FindFriend(string search){
            User searchedfor = _context.Users.FirstOrDefault(u => u.Email == search);
            if(searchedfor == null){
                ViewBag.Friends = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 2);
                ViewBag.ISent = _context.Friends.Include(u => u.User).Where(t => t.UserId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
                ViewBag.Invites = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
                ViewBag.Message = $"No User exists with Email {search}!";
                return View("ViewFriends");
            }
            Friend link = _context.Friends.FirstOrDefault(u => u.UserId == (int)HttpContext.Session.GetInt32("LoggedUser") && u.TargetId == searchedfor.UserId);
            //send a friend request via email
            User CurrentUser = _context.Users.First(u => u.UserId ==(int)HttpContext.Session.GetInt32("LoggedUser"));
            if(link == null){
                SendInvite(CurrentUser, searchedfor);
                Friend NewFriend = new Friend(){ UserId = (int)HttpContext.Session.GetInt32("LoggedUser"), TargetId = searchedfor.UserId};
                ViewBag.Friends = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 2);
                ViewBag.ISent = _context.Friends.Include(u => u.User).Where(t => t.UserId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
                ViewBag.Invites = _context.Friends.Include(u => u.User).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status == 1);
                ViewBag.Message = $"Email Invite sent to {search}!";
                _context.Add(NewFriend);
                _context.SaveChanges();
                return View("ViewFriends");
            }
            return RedirectToAction("ViewFriends");
        }
        [HttpGet("confirm/{id}")]
        public IActionResult Confirm(int id){
            Friend NewFriend = new Friend(){ UserId = (int)HttpContext.Session.GetInt32("LoggedUser"), TargetId = id};
            NewFriend.Status = 2;
            Friend Accepted = _context.Friends.FirstOrDefault(u => u.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && u.UserId == id);
            Accepted.Status = 2;
            _context.Add(NewFriend);
            _context.SaveChanges();
            return RedirectToAction("ViewFriends");
        }
                public Dictionary<DateTime,int> BestDate() // this will return null if the user isn't logged in.
        {
            List<DateTime> GoodTimes = new List<DateTime>();
                foreach( Friend fr in _context.Friends.Include(u =>u.User).ThenInclude(g => g.FreeTimes).Where(t => t.TargetId == (int)HttpContext.Session.GetInt32("LoggedUser") && t.Status ==2)){
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

        public void SendInvite(User Creator, User Invitee)
        {
            MailMessage message = new MailMessage("jqzeventreminders@gmail.com", Invitee.Email);
            message.Subject = $"Friend Invitation from {Creator.FirstName} {Creator.LastName}";
            message.Body = $"Hello {Invitee.FirstName}, You are have a calendar friend request from {Creator.FirstName} {Creator.LastName}. Please respond by going to http://localhost:5000/login to accept.";
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new System.Net.NetworkCredential("jqzeventreminders@gmail.com", "JQZEvents");
            client.EnableSsl = true;  
            client.UseDefaultCredentials = false;  
            client.Credentials = basicCredential1;  
            try   
            {  
                client.Send(message);
                Console.WriteLine("sent friend request email.");
            }               
            catch (Exception ex)
            {  
                Console.WriteLine(ex);
            }
        }
    }
}
