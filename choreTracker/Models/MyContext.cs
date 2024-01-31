#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace choreTracker.Models;

public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; } 
    public DbSet<Job> Jobs { get; set; } 
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Creator)
                .WithMany(u => u.CreatedJobs)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Worker)
                .WithMany(u => u.MyJobs)
                .HasForeignKey(j => j.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
}
