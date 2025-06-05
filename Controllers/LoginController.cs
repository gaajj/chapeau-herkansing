using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChapeauHerkansing.Controllers
{

    public class LoginController : Controller
    {
        private readonly StaffService _staffService;

        public LoginController(StaffService staffService)
        {
            _staffService = staffService;
        }

        // Deze methode toont de loginpagina
        [HttpGet]
        public IActionResult Index() // controleer je hier eerst: “Is de gebruiker al ingelogd? Zo ja, dan gaan we niet opnieuw de login-pagina tonen, maar redirecten we op basis van zijn/haar rol.
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                Claim? roleClaim = User.FindFirst(ClaimTypes.Role); // haalt de rol van de gebruiker op uit de claims
                if (roleClaim != null && Enum.TryParse<Role>(roleClaim.Value, out Role roleEnum)) // probeert de rol te parsen naar de enum Role
                {
                    return RedirectBasedOnRole(roleEnum.ToString()); // leidt door op basis van de rol van de gebruiker
                }
                return RedirectBasedOnRole(string.Empty);
            }
            return View();
        }

        // Deze methode verwerkt de login en leidt door op basis van rol
        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Username) || string.IsNullOrWhiteSpace(input.Password))
            {
                ViewBag.Error = "Please fill in a username and password.";
                return View();
            }

            Staff? staff = _staffService.GetByUsername(input.Username);
            if (staff == null || !BCrypt.Net.BCrypt.Verify(input.Password, staff.Password))
            {
                ViewBag.Error = "Invalid Credentials!";
                return View();
            }

            IList<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
        new Claim(ClaimTypes.Name, staff.Username),
        new Claim(ClaimTypes.Role, staff.Role.ToString())
    };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectBasedOnRole(staff.Role.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // laat ASP.NET Core de auth-cookie verwijderen
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        // Deze methode bepaalt de redirect op basis van gebruikersrol
        private IActionResult RedirectBasedOnRole(string role) => role switch
        {
            nameof(Role.Waiter) => RedirectToAction("Index", "TableOverview"),
            nameof(Role.Barman) or nameof(Role.Chef) => RedirectToAction("Index", "Bar_Kitchen"),
            nameof(Role.Manager) => RedirectToAction("Index", "Management"),
            _ => RedirectToAction("Index", "Login")
        };
    }
}
