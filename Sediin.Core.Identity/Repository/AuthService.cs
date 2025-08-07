using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace Sediin.Core.Identity.Repository
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<SediinIdentityUser> _signInManager;
        private readonly UserManager<SediinIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public event SendMail OnSendMailRecoveryPassword;
        public event SendMail OnSendMailConfermaEmail;

        public AuthService(
            SignInManager<SediinIdentityUser> signInManager,
            UserManager<SediinIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<SignInResult> LoginAsync(string email, string password, bool rememberMe) =>
            _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();

            _httpContextAccessor.HttpContext?.Session?.Clear();

            // Cancella i cookie
            _httpContextAccessor.HttpContext?.Response?.Cookies?.Delete(".AspNetCore.Identity.Application");
        }

        public async Task RecoveryPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                ?? throw new Exception("Indirizzo email non trovato");

            if (!user.LockoutEnabled)
                throw new Exception("Utente bloccato, non è possibile recuperare la password. Rivolgersi al fondo.");

            if (await IsEmailConfirmed(user))
            {
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                OnSendMailRecoveryPassword?.Invoke(user.Email, user.Id, code, user.Nome, user.Cognome);
            }
        }

        private async Task<bool> IsEmailConfirmed(SediinIdentityUser user)
        {
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                OnSendMailConfermaEmail?.Invoke(user.Email, user.Id, code, user.Nome, user.Cognome);
                return false;
            }
            return true;
        }

        public async Task ResetPassword(string username, string token, string password)
        {
            var user = await _userManager.FindByNameAsync(username)
                ?? throw new Exception("Username o Token errato");

            var result = await _userManager.ResetPasswordAsync(user, token, password);
            if (!result.Succeeded)
            {
                var errors = new StringBuilder();
                foreach (var err in result.Errors)
                    errors.Append(err.Description).Append("; ");
                throw new Exception(errors.ToString().TrimEnd(' ', ';'));
            }
        }

        public async Task CreateUser(string username, string email, string nome, string cognome, string rolename)
        {
            if (await _userManager.FindByNameAsync(username) != null)
                throw new Exception("Username già esistente");

            if (Debugger.IsAttached)
            {
                foreach (var _rolename in Enum.GetValues(typeof(Roles)))
                {
                    if (await _roleManager.FindByNameAsync(_rolename.ToString()) == null)
                    {
                        await _roleManager.CreateAsync(new IdentityRole { Name = rolename, NormalizedName = _rolename.ToString() });
                    }
                }
            }

            var role = await _roleManager.FindByNameAsync(rolename)
                ?? throw new Exception($"Il ruolo '{rolename}' non esiste.");

            var user = new SediinIdentityUser
            {
                UserName = username,
                NormalizedUserName = username.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                Nome = nome,
                Cognome = cognome
            };

            // Genera password casuale
            var parts = Guid.NewGuid().ToString().Split("-");
            var password = parts.First().ToUpper() + "!." + parts.Last();

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                throw new Exception("Non è stato possibile creare l'utente");

            var roleResult = await _userManager.AddToRoleAsync(user, rolename);
            if (!roleResult.Succeeded)
                throw new Exception("Non è stato possibile assegnare il ruolo all'utente");

            // Conferma email: invia codice se non confermata
            await IsEmailConfirmed(user);
        }

        public async Task ConfirmEmail(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new Exception("Utente inesistente");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
                throw new Exception("Token errato. Email non confermata.");
        }

        public async Task<(IList<SediinIdentityUser> Users, int TotalCount)> GetUsersPagedAsync(UtentiRicercaVM filtri, int pageNumber, int pageSize)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Username))
            {
                query = query.Where(x => x.UserName.Contains(filtri.UtentiRicercaVM_Username));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Nome))
            {
                query = query.Where(x => x.Nome.Contains(filtri.UtentiRicercaVM_Nome));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Cognome))
            {
                query = query.Where(x => x.Cognome.Contains(filtri.UtentiRicercaVM_Cognome));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Email))
            {
                query = query.Where(x => x.Email.Contains(filtri.UtentiRicercaVM_Email));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<IList<SediinIdentityUser>> GetAllUsersAsync(UtentiRicercaVM filtri)
        {
            var query = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Username))
            {
                query = query.Where(x => x.UserName.Contains(filtri.UtentiRicercaVM_Username));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Nome))
            {
                query = query.Where(x => x.Nome.Contains(filtri.UtentiRicercaVM_Nome));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Cognome))
            {
                query = query.Where(x => x.Cognome.Contains(filtri.UtentiRicercaVM_Cognome));
            }

            if (!string.IsNullOrEmpty(filtri.UtentiRicercaVM_Email))
            {
                query = query.Where(x => x.Email.Contains(filtri.UtentiRicercaVM_Email));
            }

            return await query.ToListAsync();
        }

        public async Task<string> GetUserRole(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles != null)
                {
                    return roles.FirstOrDefault();
                }
            }

            return null;
        }
    }
}
