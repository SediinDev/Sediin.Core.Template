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

        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
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

            await IsEmailConfirmed(user);

            //se IsEmailConfirmed non va in throw, vuol dire che email confermata
            // Inviare un messaggio di posta elettronica con questo collegamento
            string code = await _userManager.GeneratePasswordResetTokenAsync(user);
            //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

            NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
            c.Add("userId", user.Id);
            c.Add("code", code);
            /*
            var callbackUrl = $"{UriPortale("ResetPassword", "Account")}?{c.ToString()}";

            RecuperoPasswordConfermaModel _resultModel = new RecuperoPasswordConfermaModel
            {
                UrlConferma = callbackUrl,
                Email = user.Email,
                Cognome = user.Cognome,
                Nome = user.Nome,
                Username = user.UserName
            };

            await UserManager.SendEmailAsync(user.Id, "Recupero Password", RenderTemplate("Account/ForgotPassword_Mail", _resultModel));
            var _html = RenderTemplate("Account/ForgotPassword", _resultModel);

            return Content(_html);

            return ""
            */
        }

        /// <summary>
        ///  verifica indirizzo email e confermato, se no, invia nuovo codice
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="Exception"></exception>
        private async Task IsEmailConfirmed(IdentityUser user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // var callbackUrl = Url.Action("ConfirmEmail", "Registrazione", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                c.Add("userId", user.Id);
                c.Add("code", code);

                //var callbackUrl = $"{UriPortale("ConfirmEmail", "Account")}?{c.ToString()}";


                //RegistrazioneConfermaModel _resultModel = new RegistrazioneConfermaModel
                //{
                //    UrlConferma = callbackUrl,
                //    Email = user.Email,
                //    Cognome = user.Cognome,
                //    Nome = user.Nome,
                //    Username = user.UserName
                //};

                //await _userManager.SendEmailAsync(user.Id, "Conferma account", RenderTemplate("Registrazione/Confirm_Mail", _resultModel));

                //var _html = RenderTemplate("Registrazione/Confirm", _resultModel);

                throw new Exception("Email inviata per confermare l'utenza"/*_html*/);
            }
        }

    }
}
