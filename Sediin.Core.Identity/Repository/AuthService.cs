using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Abstract;
using System.Collections.Specialized;
using System.Web;

namespace Sediin.Core.Identity.Repository
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        private readonly UserManager<IdentityUser> _userManager;

        public event SendMail OnSendMailRecoveryPassword;

        public event SendMail OnSendMailConfermaEmail;

#pragma warning disable
        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task RecoveryPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Indirizzo email non trovato");
            }

            if (user.LockoutEnabled == false)
            {
                throw new Exception("Utente bloccato, non è possibile recuperare la password. Rivolgersi al fondo.");
            }

            //se IsEmailConfirmed, viene mandata email per cambio password
            if (await IsEmailConfirmed(user))
            {
                // Inviare un messaggio di posta elettronica
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);

                OnSendMailRecoveryPassword?.Invoke(user.Email, user.Id, code);
            }
        }

        /// <summary>
        ///  verifica indirizzo email e confermato, se no, invia nuovo codice
        ///  OnSendMailConfermaEmail
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="Exception"></exception>
        private async Task<bool> IsEmailConfirmed(IdentityUser user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                OnSendMailConfermaEmail?.Invoke(user.Email, user.Id, code);

                return false;
            }

            return true;
        }
    }
}
