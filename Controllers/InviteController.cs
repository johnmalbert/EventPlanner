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
    public class InviteController : Controller
    {
        private readonly MyContext _context;

        public InviteController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("invite/{EventId}/{UserToInviteId}")]
        public IActionResult SendInvite(int EventId, int UserToInviteId)
        {
            User CurrentUser = LoggedUser();
            if(CurrentUser==null)
                return Redirect("/Logout");
            Console.WriteLine("Send an invite to user with Id" + UserToInviteId);
            User UserToInvite = _context.Users.FirstOrDefault(u => u.UserId == UserToInviteId);
            Event EventToInvite = _context.Events.FirstOrDefault(e => e.EventId == EventId);
            Console.WriteLine($"Inviting {UserToInvite.Email} to {EventToInvite.Title}");

            //create a new Invite
            Invite newInvite = new Invite{
                UserId = CurrentUser.UserId,
                TargetId = UserToInvite.UserId,
                EventId = EventToInvite.EventId
            };
            RequestedInvite newRequest = new RequestedInvite{
                UserId = UserToInvite.UserId,
                Requester = CurrentUser.UserId,
                EventId = EventToInvite.EventId,
            };
            
            _context.Add(newInvite);
            _context.Add(newRequest);
            _context.SaveChanges();
            //create a new email
            SendInvite(CurrentUser,UserToInvite,EventToInvite);
            return Redirect("/Dashboard");
        }
        public void SendInvite(User Creator, User Invitee, Event Event)
        {
            MailMessage message = new MailMessage("jqzeventreminders@gmail.com", Invitee.Email);
            message.Subject = $"Event Invitation from {Creator.FirstName} {Creator.LastName}";
            message.Body = $"Hello {Invitee.FirstName}, You are being invited to the event \"{Event.Title}\", on {Event.ScheduledAt}. Please respond by going to http://localhost:5000/login to accept.";
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
            }               
            catch (Exception ex)
            {  
                Console.WriteLine(ex);
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
    }
}