using System;
using System.ComponentModel.DataAnnotations;

namespace EventPlanner.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        [Required(ErrorMessage="Please enter your name.")]
        public string Creator { get; set; }

        [Required(ErrorMessage="Please enter a message")]
        public string Note { get; set; }

        public int EventId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}