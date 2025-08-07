using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;

namespace Sediin.Core.Identity.Abstract
{
    public delegate Task SendMail(string email, string userId, string token, string nome, string cognome);

    public interface IAuthService
    {
        event SendMail OnSendMailConfermaEmail;

        event SendMail OnSendMailRecoveryPassword;
        
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        
        Task LogoutAsync();

        /// <summary>
        /// Recovery Password, se Email non e confermata, deve essere invocato
        /// OnSendMailConfermaEmail
        /// altrimenti se confermata invocare
        /// OnSendMailRecoveryPassword
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task RecoveryPassword(string email);

        Task ResetPassword(string username, string token, string password);

        Task CreateUser(string username, string email, string nome, string cognome, string role);
        Task ConfirmEmail(string userId, string code);

        Task<string> GetUserRole(string userId);
        Task<SediinIdentityUser> GetUserByUsername(string userName);
        Task<SediinIdentityUser> GetUserById(string id);
        Task ChangePassword(string userName, string currentPassword, string newPassword);

        Task<(IList<SediinIdentityUser> Users, int TotalCount)> GetUsersPagedAsync(UtentiRicercaVM filtri , int pageNumber, int pageSize);
        Task<IList<SediinIdentityUser>> GetAllUsersAsync(UtentiRicercaVM filtri);
    }
}
