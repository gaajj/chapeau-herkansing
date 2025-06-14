using System.Security.Claims;
using ChapeauHerkansing.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ChapeauHerkansing.Services.Interfaces;


public class AuthService : IAuthService
{
    private readonly IStaffRepository repo;
    private readonly IHttpContextAccessor ctx;

    public AuthService(IStaffRepository repo, IHttpContextAccessor ctx)
    {
        this.repo = repo;      // geeft toegang tot de medewerkers-tabel
        this.ctx = ctx;       // nodig om de cookie te kunnen zetten
    }

    public async Task<ClaimsPrincipal?> TryLoginAsync(string username, string password)
    {
        // Zoek de medewerker op basis van username
        var staff = repo.GetStaffByUsername(username);

        // 2. Bestaat hij niet of klopt het wachtwoord niet? -> login mislukt
        if (staff == null || !BCrypt.Net.BCrypt.Verify(password, staff.Password))
            return null;

       // Bouw de claims: hier staat wie je bent en welke rol je hebt
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, staff.Id.ToString()),
            new Claim(ClaimTypes.Name,         staff.Username),
            new Claim(ClaimTypes.Role,         staff.Role.ToString())
        };

        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

         // Zet de auth-cookie, zo blijft de gebruiker ingelogd
        await ctx.HttpContext!.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return principal;      // Handig voor de controller om meteen te weten wie het is
    }

    public Task SignOutAsync() =>
        // Verwijdert de auth-cookie: gebruiker is uitgelogd
        ctx.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
