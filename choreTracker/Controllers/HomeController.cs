using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


using choreTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace choreTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context )
    {   
        _context = context;
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if (_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already used .");
                return View("Index");
            }
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            newUser.Password = hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("userId", newUser.UserId);
            HttpContext.Session.SetString("name", newUser.FName);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }
    [HttpPost]
    public IActionResult Login(LogUser newLogUser)
    {
        if (ModelState.IsValid)
        {
            User userfromdb = _context.Users.FirstOrDefault(u => u.Email == newLogUser.LogEmail);
            if (userfromdb == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                return View("Index");
            }
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(newLogUser, userfromdb.Password, newLogUser.LogPassword);
            if (result == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                return View("Index");
            }
            HttpContext.Session.SetInt32("userId", userfromdb.UserId);
            HttpContext.Session.SetString("name", userfromdb.FName);
            return RedirectToAction("Dashboard");
        }
        return View("Index");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        List<Job> AllJobs = _context.Jobs.Include(c=>c.Creator).ToList();

        return View(AllJobs);
    }

    [HttpGet("addjob")]
    public IActionResult AddJob()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }
    [HttpPost("jobs/create")]
    public IActionResult CreateJob(Job newJob)
    {
        if (ModelState.IsValid)
        {
                _context.Add(newJob);
                _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        return View("AddJob", newJob);
    }
    [HttpGet("viewJob/{JobId}")]
    public IActionResult ViewJob(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        return View();
    }
    [HttpGet("edit/{JobId}")]
    public IActionResult EditJob(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
        return RedirectToAction("Index");
        }
            Job? jobToEdit = _context.Jobs
            .SingleOrDefault(d => d.JobId == JobId);
        return View( jobToEdit);
    }
     [HttpPost("/jobs/update/{JobId}")]
    public IActionResult UpdateJob(int JobId, Job newestJob)
    {
        Job? oldJob = _context.Jobs.FirstOrDefault(b => b.JobId ==JobId);
         if (ModelState.IsValid)
        {
            oldJob.Title = newestJob.Title;
            oldJob.Description = newestJob.Description;
            oldJob.Location = newestJob.Location;
            oldJob.UpdatedAt = newestJob.UpdatedAt;
            _context.SaveChanges();

            return RedirectToAction("Dashboard",newestJob);
        }
        return View("EditJob", oldJob);

    }
   
    [HttpPost("delete/{JobId}")]
    public IActionResult DeleteJob(int JobId)
    {
        System.Console.WriteLine(JobId);
            Job toDelete = _context.Jobs.FirstOrDefault(d =>d.JobId == JobId);
            System.Console.WriteLine(toDelete==null);
            if(toDelete == null)
                return RedirectToAction("Dashboard");
            _context.Jobs.Remove(toDelete);
             _context.SaveChanges();
            return RedirectToAction("Dashboard");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
