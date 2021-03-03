using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage="Please Provide your first name.")]
        [MinLength(2, ErrorMessage="Please ensure that your first name is at least two characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Please Provide your last name.")]
        [MinLength(2, ErrorMessage="Please ensure that your last name is at least two characters")]
        public string LastName { get; set; }

        //validate the address somehow
        // [AddressAttribute(ErrorMessage="Please provide a zipcode.")]
        [MinLength(5, ErrorMessage="Address name must be at least five characters")]
        public string Address { get; set;}

        [Required(ErrorMessage="Please Provide your email address.")]
        [EmailAddress(ErrorMessage="Please provide a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage="Please Provide your email address.")]
        [MinLength(8, ErrorMessage="Password must be at least eight characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage="Password and Confirmation must match.")]
        [DataType(DataType.Password)]
        public string Confirm {get; set;}

        // One-to-Many relationship showcasing when the User has time available
        public List<Time> FreeTimes {get;set;}

        //Many-to-Many relationship between users
        public List<Friend> Friends {get;set;}

        public List<Reminder> Reminders { get; set; }

        public List<Event> createdEvents {get;set;}
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
    }
} 