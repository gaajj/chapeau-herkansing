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
      
        return User.Identity?.IsAuthenticated == true
            ? RedirectBasedOnRole(User.FindFirstValue(ClaimTypes.Role))
            : View();   
    }

  
    [HttpPost]
    public async Task<IActionResult> Index(LoginViewModel vm)
    {
      
        if (string.IsNullOrWhiteSpace(vm.Username) || string.IsNullOrWhiteSpace(vm.Password))
        {
            ViewBag.Error = "Username en password verplicht";
            return View();
        }
         
        
        var principal = await auth.TryLoginAsync(vm.Username, vm.Password);

        if (principal == null)   
        {
            ViewBag.Error = "Invalid credentials";
            return View();
        }

        
        return RedirectBasedOnRole(principal.FindFirstValue(ClaimTypes.Role));
    }

    // POST /Login/Logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await auth.SignOutAsync();       
        return RedirectToAction(nameof(Index)); // terug naar de loginpagina
    }

    
    private IActionResult RedirectBasedOnRole(string? role) => role switch
    {
        nameof(Role.Waiter) => RedirectToAction("Index", "TableOverview"),
        nameof(Role.Barman) or nameof(Role.Chef) => RedirectToAction("Index", "Bar_Kitchen"),
        nameof(Role.Manager) => RedirectToAction("Index", "Management"),
        _ => RedirectToAction(nameof(Index))
    };
}
