using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class Invite
    {
        [Key]
        public int InviteId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int TargetId { get; set; }
        public int EventId {get;set;}
        public Event Event {get;set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}