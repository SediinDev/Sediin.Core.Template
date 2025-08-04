using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Abstract;

namespace Sediin.Core.Identity.Repository
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

            //if (result.Succeeded)
            //{
            //    //_logger.LogInformation("User logged in.");
            //    return LocalRedirect(returnUrl);
            //}

            //if (result.RequiresTwoFactor)
            //{
            //    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            //}
            
            //if (result.IsLockedOut)
            //{
            //    throw new Exception("User account locked out.");
            //}
            //else
            //{
            //    throw new Exception("Invalid login attempt.");
            //}
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
