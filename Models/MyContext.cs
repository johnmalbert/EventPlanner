using Microsoft.EntityFrameworkCore;
namespace EventPlanner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events {get;set;}
        public DbSet<Time> Times {get;set;}
        public DbSet<Friend> Friends {get;set;}
        public DbSet<Reminder> Reminders {get; set;}
        //add events

        // add other models
        
    }
}