using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.ViewModels.Login;
using ChapeauHerkansing.Models.Enums;


public class LoginController : Controller
{
    private readonly IAuthService auth;

    public LoginController(IAuthService auth) => this.auth = auth;

    // GET /Login/Index
    public IActionResult Index()
    {
        // Als je al ingelogd bent: direct doorsturen naar de juiste pagina
        return User.Identity?.IsAuthenticated == true
            ? RedirectBasedOnRole(User.FindFirstValue(ClaimTypes.Role))
            : View();   // anders: gewoon het login-formulier tonen
    }

    // POST /Login/Index
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel vm)
    {
        // Basisvalidatie: lege velden tegenhouden
        if (string.IsNullOrWhiteSpace(vm.Username) || string.IsNullOrWhiteSpace(vm.Password))
        {
            ViewBag.Error = "Username en password verplicht";
            return View();
        }
         
        // Laat de AuthService het zware werk doen
        var principal = await auth.TryLoginAsync(vm.Username, vm.Password);

        if (principal == null)   // Mislukt? Terug naar login met foutmelding
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // Gelukt: doorschakelen op basis van rol
        return RedirectBasedOnRole(principal.FindFirstValue(ClaimTypes.Role));
    }

    // POST /Login/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await auth.SignOutAsync();       // Cookie weg, klaar
        return RedirectToAction(nameof(Index));
    }

    // Bepaalt welke pagina bij welke rol hoort
    private IActionResult RedirectBasedOnRole(string? role) => role switch
    {
        nameof(Role.Waiter) => RedirectToAction("Index", "TableOverview"),
        nameof(Role.Barman) or nameof(Role.Chef) => RedirectToAction("Index", "Bar_Kitchen"),
        nameof(Role.Manager) => RedirectToAction("Index", "Management"),
        _ => RedirectToAction(nameof(Index))
    };
}
