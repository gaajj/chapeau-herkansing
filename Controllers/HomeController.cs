using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
   private readonly IRepository<Staff> _repo;

    public HomeController(ILogger<HomeController> logger, IRepository<Staff> repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public IActionResult Index()
    {
        var users = _repo.GetAll();
        return View(users);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
