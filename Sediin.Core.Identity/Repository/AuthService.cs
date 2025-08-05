using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Entities;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Sediin.Core.Identity.Repository
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<SediinIdentityUser> _signInManager;

        private readonly UserManager<SediinIdentityUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public event SendMail OnSendMailRecoveryPassword;

        public event SendMail OnSendMailConfermaEmail;

#pragma warning disable
        public AuthService(SignInManager<SediinIdentityUser> signInManager, UserManager<SediinIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

                OnSendMailRecoveryPassword?.Invoke(user.Email, user.Id, code, user.Nome, user.Cognome);
            }
        }

        /// <summary>
        ///  verifica indirizzo email e confermato, se no, invia nuovo codice
        ///  OnSendMailConfermaEmail
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="Exception"></exception>
        private async Task<bool> IsEmailConfirmed(SediinIdentityUser user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                OnSendMailConfermaEmail?.Invoke(user.Email, user.Id, code, user.Nome, user.Cognome);

                return false;
            }

            return true;
        }

        public async Task ResetPassword(string username, string token, string password)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                throw new Exception("Username o Token errato");
            }

            var result = await _userManager.ResetPasswordAsync(user, token, password);

            if (!result.Succeeded)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    stringBuilder.Append(item.Description + ", ");
                }

                throw new Exception(stringBuilder.ToString());
            }

        }

        public async Task CreateUser(string username, string email, string nome, string cognome, string rolename)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
                throw new Exception("Username già esistente");
            }

            var role = await _roleManager.FindByNameAsync(rolename);

            if (role == null)
            {
                throw new Exception("Il ruolo " + rolename + " non esiste.");
            }

            SediinIdentityUser sediinIdentityUser = new SediinIdentityUser();
            sediinIdentityUser.Email = email;
            sediinIdentityUser.UserName = username;
            sediinIdentityUser.NormalizedUserName = username;
            sediinIdentityUser.NormalizedEmail = email;
            sediinIdentityUser.Cognome = cognome;
            sediinIdentityUser.Nome = nome;

            string[] _p = Guid.NewGuid().ToString().Split("-");
            string password = _p.FirstOrDefault().ToUpper() + "!." + _p.LastOrDefault();

            var _newuser = await _userManager.CreateAsync(sediinIdentityUser, password);

            if (!_newuser.Succeeded)
            {
                throw new Exception("Non e stato possibile creare utente");
            }

            var _userole = await _userManager.AddToRoleAsync(sediinIdentityUser, rolename);

            if (!_userole.Succeeded)
            {
                throw new Exception("Non e stato possibile aggiungere utente al ruolo");
            }

            var _usercreated = await _userManager.FindByNameAsync(username);

            await IsEmailConfirmed(_usercreated);

        }

        public async Task ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Utente inesistente");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
           
            if (!result.Succeeded)
            {
                throw new Exception("Token errato. Email non confermata.");
            }
        }
    }
}
