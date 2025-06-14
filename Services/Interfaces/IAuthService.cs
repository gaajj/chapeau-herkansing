using System.Security.Claims;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ClaimsPrincipal?> TryLoginAsync(string username, string password);
        Task SignOutAsync();
    }
}
