using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Models
{
    public class LoginUser
    {
        [Key]
        public int LoginUserId { get; set; }


        [Required(ErrorMessage="Please provide your email.")]
        [EmailAddress(ErrorMessage="Please provide a valid email.")]
        public string Email { get; set; }

        [Required(ErrorMessage="Please provide your password.")]
        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }

    }
}