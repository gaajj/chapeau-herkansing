using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System.Collections.Generic;

namespace ChapeauHerkansing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStaffRepository _staffRepository;

        public HomeController(ILogger<HomeController> logger, IStaffRepository staffRepository)
        {
            _logger = logger;
            _staffRepository = staffRepository;
        }

        public IActionResult Index()
        {
            // Voor demo toon medewerkers
            List<Staff> staffList = _staffRepository.GetAllStaff();
            return View(staffList); // Alleen als View /Views/Home/Index.cshtml dat ondersteunt
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
}
