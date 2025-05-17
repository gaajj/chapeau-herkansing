// Controllers/LoginController.cs
using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Http;

namespace ChapeauHerkansing.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsersRepository _userRepository;

        public LoginController(IUsersRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Deze methode toont de loginpagina
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Deze methode verwerkt de login en leidt door op basis van rol
        [HttpPost]
        [HttpPost]
        public IActionResult Index(User input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
            {
                ViewBag.Error = "Please fill in an username and password.";
                return View();
            }

            var user = _userRepository.GetByUsername(input.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                ViewBag.Error = "Invalid Credintials!";
                return View();
            }

            Response.Cookies.Append("UserId", user.Id.ToString());
            Response.Cookies.Append("Username", user.Username);
            Response.Cookies.Append("Role", user.Role);
            return RedirectBasedOnRole(user.Role);
        }


        // Deze methode logt een gebruiker uit
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("Role");
            return RedirectToAction("Index", "Login");
        }

        // Deze methode bepaalt de redirect op basis van gebruikersrol
        private IActionResult RedirectBasedOnRole(string role)
        {
            return role switch
            {
                "Waiter" => RedirectToAction("Index", "TableOverview"),
                "Bar" or "Kitchen" => RedirectToAction("Index", "Kitchen"),
                "Manager" => RedirectToAction("Index", "Management"),
                _ => RedirectToAction("Index", "Login")
            };
        }
    }
}
