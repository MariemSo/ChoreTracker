#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace choreTracker.Models;

public class User{

    [Key]
    public int UserId {get;set;}
    [Required(ErrorMessage ="First Name is required")]
    public string FName{get;set;}
    
    [Required(ErrorMessage ="Last Name is required")]
    public string LName{get;set;}

    [Required]
    [EmailAddress]
    public string Email{get;set;}

    [Required]
    [DataType(DataType.Password)]
    [MinLength(8,ErrorMessage="The password must be at least 8 characters")]
    public string Password{get;set;}

    [Required(ErrorMessage ="Confirm password is required")]
    [NotMapped]
    [Compare("Password",ErrorMessage ="Password and confirm password must match !")]
    [DataType(DataType.Password)]
    public string ConfirmPassword{get;set;}
    
    public DateTime CreatedAt{get;set;}=DateTime.Now;
    public DateTime UpdatedAt{get;set;}=DateTime.Now;

    public List<Job> CreatedJobs{get;set;}=new List<Job>();
    public List<Job> MyJobs{get;set;}=new List<Job>();



}