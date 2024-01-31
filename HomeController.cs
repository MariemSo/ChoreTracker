using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using choreTracker.Models;
using AspNetCore;

namespace choreTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

   //   -------------- LandingPage / LogReg Form ---------

    public IActionResult Index()
    {
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
        return View("Index");
    }
    //   -------------- Login Job---------

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
        ViewBag.AllJobs=_context.Jobs.ToList();
        // you can take the list of "myJobs" from this model"
        User JobsUser= _context.Users.FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("userId"));
        
        return View(JobsUser);
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
            return RedirectToAction("Index")
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
        Job? oneJob = _context.Jobs.FirstOrDefault(j =>j.JobId == JobId);
        return View(oneJob);
    }
//   -------------- Add Job To myJobs---------
    [HttpGet("addTomyJobs/{id}")]
    public IActionResult AddToFavorites(int id)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
    // User LoggedUser=_context.User.FirstOrDefault(u=>u.UserId==HttpContext.Session.GetInt32("userId"));
    // bool alreadyInmyJobs= LoggedUser.myJobs.Any(j=>j.JobId==id);
    Job TheJob=_context.Jobs.FirstOrDefault(j=>j.JobId==id);
    if(TheJob.WorkerId==null){
        TheJob.WorkerId=HttpContext.Session.GetInt32("userId");
        _context.Jobs.SaveChanges();
        return RedirectToAction("Dashboard");
    }
    return RedirectToAction("ViewJob",new {id});
    }

//   -------------- Delete Job---------
    [HttpPost("delete/{JobId}")]
    public IActionResult DeleteJob(int JobId)
    {   
        Job JobToDelete = _context.Jobs.FirstOrDefault(d =>d.JobId == JobId);
        if((JobToDelete.UserId==HttpContext.Session.GetInt32("userId") )|| (JobToDelete.WorkerId==HttpContext.Session.GetInt32("userId"))){
            _context.Jobs.Remove(JobToDelete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        return RedirectToAction("Index");
            
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
