using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Time
    {
        [Key]
        public int TimeId {get;set;}

        public DateTime StartAt {get;set;}

        public int UserId {get;set;}
        public User User {get;set;}
    }
}