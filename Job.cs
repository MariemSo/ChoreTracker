#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace choreTracker.Models;

public class Job{

    [Key]
    public int JobId {get;set;}
    
    [MinLength(3, ErrorMessage = "The minimum length for the Title field is 3 characters")]
    public string Title{get;set;}

    [MinLength(10, ErrorMessage = "The minimum length for the Description field is 10 characters")]
    public string  Description{get;set;}
    
    [MinLength(1, ErrorMessage = "Location must not be blank")]
    public string Location{get;set;}

    public DateTime CreatedAt{get;set;}=DateTime.Now;
    public DateTime UpdatedAt{get;set;}=DateTime.Now;

    public int UserId {get;set;}
    public User? Creator {get;set;}


    public int? WorkerId{get;set;}=null
    public User? Worker {get;set;}


}