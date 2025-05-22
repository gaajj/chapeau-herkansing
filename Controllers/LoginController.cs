// Controllers/LoginController.cs
using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;


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
        public async Task<IActionResult> Index(User input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
            {
                ViewBag.Error = "Please fill in an username and password.";
                return View();
            }

            var user = _userRepository.GetByUsername(input.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(input.Password, user.Password))
            {
                ViewBag.Error = "Invalid Credentials!";
                return View();
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectBasedOnRole(user.Role);
        }



        [HttpPost] 
        public async Task<IActionResult> Logout()
        {
            // laat ASP.NET Core de auth-cookie verwijderen
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        // Deze methode bepaalt de redirect op basis van gebruikersrol
        private IActionResult RedirectBasedOnRole(string role)
        {
            return role switch
            {
                "Waiter" => RedirectToAction("Index", "TableOverview"),
                "Bar" or "Kitchen" => RedirectToAction("Index", "Bar_Kitchen"),
                "Manager" => RedirectToAction("Index", "Management"),
                _ => RedirectToAction("Index", "Login")
            };
        }
    }
}
