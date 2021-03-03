using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class Reminder
    {
        [Key]
        public int ReminderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string MessageBody { get; set; }
        public string MesssageSubject { get; set; }
        public string to { get; set; } // the user's email address
        public string from { get; set; } = "jqzeventreminders@gmail.com";
        public string PW { get; set; } = "JQZEvents"; // the pw for jqzeventreminders@gmail.com
        public DateTime TimeToSendReminder { get; set; } // when the reminder will be sent
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}