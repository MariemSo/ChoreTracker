#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace choreTracker.Models;

public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; } 
    public DbSet<Job> Jobs { get; set; } 
    public DbSet<Favorite> Favorites { get; set; } 
}
