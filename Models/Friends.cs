using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class Friend
    {
        [Key]
        public int FriendId {get;set;}

        public int UserId {get;set;}
        public User User {get;set;}
        public int TargetId {get;set;}
    
        // Status will be to show whether the link is an invite
        // or if the link is accepted. (I.e. 1 = invite, 2 = accepted, 0 = declined)
        // Status is 1 by default as all links start as invite
        public int Status {get;set;} = 1;
    }
}