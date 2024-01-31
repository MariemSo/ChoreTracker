#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace choreTracker.Models;
public class Favorite
{
    [Key]
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int JobId { get; set; }

    public User? User { get; set; }
    public Job? Job { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
}
