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

        public User User1 {get;set;}
        public int User1Id {get;set;}


        public User User2 {get;set;}
        public int User2Id {get;set;}
    
        // Status will be to show whether the link is an invite
        // or if the link is accepted. (I.e. 1 = invite, 2 = accepted, 0 = declined)
        // Status is 1 by default as all links start as invite
        public int Status {get;set;} = 1;
    }
}