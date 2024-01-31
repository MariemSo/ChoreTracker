using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


using choreTracker.Models;

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

//        //   -------------- LandingPage / LogReg Form ---------

    public IActionResult Index()
    {
        HttpContext.Session.SetInt32("mbr", value: 2);
        return View();
    }
    [HttpPost]
    //   -------------- Register Job---------

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
        HttpContext.Session.SetInt32("mbr", value: 1);

        return View("Index");
    }
    //   -------------- Login Job---------

    [HttpPost]
    public IActionResult Login(LogUser newLogUser)
    {
        HttpContext.Session.SetInt32("mbr", 2);
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
//   -------------- LogOut---------

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
        ViewBag.AllJobs = _context.Jobs.Include(c => c.Creator).Where(j => j.WorkerId == null).ToList();
        int userId = HttpContext.Session.GetInt32("userId") ?? 0;
        User? jobsUser = _context.Users
            .Include(u => u.MyJobs)
            .FirstOrDefault(u => u.UserId == userId);
        return View(jobsUser);
    }
    //   -------------- ADD Job / Get---------

    [HttpGet("addjob")]
    public IActionResult AddJob()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            
            return RedirectToAction("Dashboard");
        }
        return View();
    }
    //   -------------- Create Job / Post---------

    [HttpPost("jobs/create")]
    public IActionResult CreateJob(Job newJob)
    {
        if (ModelState.IsValid)
        {
                _context.Add(newJob);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
        }
        return View("AddJob");
    }
    
    [HttpGet("edit/{JobId}")]
    public IActionResult EditJob(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
        return RedirectToAction("Index");
        }
        Job? jobToEdit = _context.Jobs.SingleOrDefault(d => d.JobId == JobId);
        return View( jobToEdit);
    }
    //   -------------- Update Job / Post---------

    [HttpPost("jobs/update/{JobId}")]
    public IActionResult UpdateJob(int JobId, Job newestJob)
    {
        Job? oldJob = _context.Jobs.FirstOrDefault(b => b.JobId ==JobId);
        if(oldJob.UserId==HttpContext.Session.GetInt32("userId")){
            if (ModelState.IsValid)
            {
                oldJob.Title = newestJob.Title;
                oldJob.Description = newestJob.Description;
                oldJob.Location = newestJob.Location;
                oldJob.UpdatedAt = newestJob.UpdatedAt;
                _context.SaveChanges();

                return RedirectToAction("Dashboard");
            }
            return View("EditJob", oldJob);
        }else{
            return RedirectToAction("Index");
        }

    }
    //   -------------- Show One Job---------

    [HttpGet("view/{JobId}")]
    public IActionResult ViewJob(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Job? oneJob = _context.Jobs.Include(u=>u.Creator).FirstOrDefault(j =>j.JobId == JobId);
        return View(oneJob);
    }
//   -------------- Add Job To myJobs---------
   [HttpPost("addTomyJobs/{JobId}")]
    public IActionResult AddToMyJobs(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }

        int currentUserId = HttpContext.Session.GetInt32("userId") ?? 0;
        Job jobToAssign = _context.Jobs.FirstOrDefault(j => j.JobId == JobId);

        if (jobToAssign != null)
        {
            jobToAssign.WorkerId = currentUserId;
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        return RedirectToAction("Dashboard"); 
    }
    //--------------Cancel From MyJobs-----------
    public IActionResult Cancel(int JobId)
{
    if (HttpContext.Session.GetInt32("userId") == null)
    {
        return RedirectToAction("Index");
    }

    int currentUserId = HttpContext.Session.GetInt32("userId") ?? 0;

    int? currentUser = HttpContext.Session.GetInt32("userId");
    Job jobToCancel = _context.Jobs.FirstOrDefault(j => j.JobId == JobId);

    if (currentUser != null && jobToCancel != null)
    {
        // Check if the current user is the assigned worker for the job
        if (jobToCancel.WorkerId == currentUserId)
        {
            jobToCancel.WorkerId = null;

            _context.SaveChanges();

            return RedirectToAction("Dashboard");
        }
        else
        {
            // Handle the case where the current user is not the assigned worker
            // You can add a message or redirect to an error page
            return RedirectToAction("Error");
        }
    }

    return RedirectToAction("Dashboard");
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
