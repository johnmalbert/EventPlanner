using Microsoft.EntityFrameworkCore;
namespace EventPlanner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }

        //add events

        // add other models
        
    }
}