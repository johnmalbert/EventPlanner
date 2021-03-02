using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Event
    {
        [Key]
        public int EventId {get;set;}

        [Required]
        public string Title {get;set;}
        [Required]
        public DateTime ScheduledAt {get;set;}
        public DateTime EndAt {get;set;}
        public string Description {get;set;}
        public int Duration {get;set;}
        // hord => hours or days
        public string HoursOrDays {get;set;}
        public string Location {get;set;}

        // public string[] Items {get;set;}
        public List<Link> Guests {get;set;}

        public int UserId {get;set;}
        public User Creator {get;set;}
    }
}
