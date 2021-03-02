using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Link
    {
        [Key]
        public int LinkId {get;set;}
        public int UserId {get;set;}
        public int EventId {get;set;}
        public User User {get;set;}
        public Event Event {get;set;}
    }
}