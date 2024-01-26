using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;


using choreTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

    //   -------------- Dashboard---------

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        int userId = (int)HttpContext.Session.GetInt32("userId");

        DashboardViewModel viewModel = new DashboardViewModel
        {
            AllJobs = _context.Jobs.Include(c => c.Creator).Include(f => f.myUsers).ThenInclude(e => e.User).ToList(),
            UserFavorites = _context.Favorites.Where(f => f.UserId == userId).ToList()
        };
            return View(viewModel);
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
        return View("AddJob", newJob);
    }
    //   -------------- Show One Job---------

    [HttpGet("viewJob/{JobId}")]
    public IActionResult ViewJob(int JobId)
    {
        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Index");
        }
        Job? oneJob = _context.Jobs.Include(c=>c.Creator).FirstOrDefault(j =>j.JobId == JobId);
        return View(oneJob);
    }
    //   -------------- Edit Job / Get ---------
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
    //   -------------- Update Job / Post---------

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
//   -------------- Add Job To Favorite---------
[HttpPost]
public IActionResult AddToFavorites(int JobId)
{
    if (HttpContext.Session.GetInt32("userId") == null)
    {
        return RedirectToAction("Index");
    }

    int userId = (int)HttpContext.Session.GetInt32("userId");

    bool alreadyInFavorites = _context.Favorites.Any(f => f.UserId == userId && f.JobId == JobId);

    if (!alreadyInFavorites)
    {
        Favorite newFavorite = new Favorite
        {
            UserId = userId,
            JobId = JobId
        };

        _context.Add(newFavorite);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    return RedirectToAction("Dashboard");
}
// ----------Done /Delete Job From Fav-------
[HttpPost("Favorite/delete")]
public IActionResult Done(int JobId)
{
    if (HttpContext.Session.GetInt32("userId") == null)
    {
        return RedirectToAction("Index");
    }

    int userId = (int)HttpContext.Session.GetInt32("userId");

    Favorite favoriteToRemove = _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.JobId == JobId);

    if (favoriteToRemove != null)
    {
        _context.Favorites.Remove(favoriteToRemove);

        Job jobToRemove = _context.Jobs.FirstOrDefault(j => j.JobId == JobId);

        if (jobToRemove != null)
        {
            _context.Jobs.Remove(jobToRemove);
        }

        _context.SaveChanges();
    }

    return RedirectToAction("Dashboard");
}


//   -------------- Delete Job---------
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
