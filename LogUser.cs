#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace choreTracker.Models;
public class LogUser{
    [Required(ErrorMessage ="The Email is required")]
    [EmailAddress]
    public string LogEmail{get;set;}

    [Required(ErrorMessage ="The Password is required")]
    [DataType(DataType.Password)]
    public string LogPassword{get;set;}

} 
