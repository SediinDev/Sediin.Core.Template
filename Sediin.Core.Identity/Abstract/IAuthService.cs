using Microsoft.AspNetCore.Identity;

namespace Sediin.Core.Identity.Abstract
{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();

        Task RecoveryPassword(string email);
    }
}
