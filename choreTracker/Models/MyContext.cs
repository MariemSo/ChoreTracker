#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace choreTracker.Models;

public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
}
