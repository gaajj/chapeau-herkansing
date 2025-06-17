using System.Security.Claims;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Models;


public class AuthService : IAuthService
{
    private readonly IStaffRepository repo;
    private readonly IHttpContextAccessor ctx;

    public AuthService(IStaffRepository repo, IHttpContextAccessor ctx)
    {
        this.repo = repo;     
        this.ctx = ctx;       
    }

    public async Task<ClaimsPrincipal?> TryLoginAsync(string username, string password)
    {
        Staff staff = repo.GetStaffByUsername(username);

        if (staff == null || !BCrypt.Net.BCrypt.Verify(password, staff.Password))
            return null;

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
            new Claim(ClaimTypes.Name,         staff.Username),
            new Claim(ClaimTypes.Role,         staff.Role.ToString())
        };
        ClaimsPrincipal principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

         
        await ctx.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return principal;      
    }

    public Task SignOutAsync() =>
        
        ctx.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); 
}
