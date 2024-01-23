using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using choreTracker.Models;

namespace choreTracker.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
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
        if ((HttpContext.Session.GetInt32("userId") == null))
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpGet("addJob")]
    public IActionResult AddJob()
    {
        if ((HttpContext.Session.GetInt32("userId") == null))
        {
            return RedirectToAction("Index");
        }
        return View();
    }
    [HttpGet("view/{id}")]
    public IActionResult ViewJob(int id)
    {
        if ((HttpContext.Session.GetInt32("userId") == null))
        {
            return RedirectToAction("Index");
        }
        return View();
    }
    [HttpGet("edit/{id}")]
    public IActionResult EditJob(int id)
    {
        if ((HttpContext.Session.GetInt32("userId") == null))
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
