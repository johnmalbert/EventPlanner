using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class RequestedInvite
    {
        [Key]
        public int RequestedInviteId { get; set; }
        public int UserId { get; set; } // this is the target
        public User User { get; set; }
        public int Requester { get; set; }
        public int EventId {get;set;}
        public Event Event {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}