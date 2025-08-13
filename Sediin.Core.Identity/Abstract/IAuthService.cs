using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Entities.DTO;
using Sediin.Core.Identity.Models;

namespace Sediin.Core.Identity.Abstract
{
    public delegate Task SendMail(string email, string userId, string username, string token, string nome, string cognome);

    public delegate Task NotifyUser(string username);

    public interface IAuthService
    {
        event SendMail OnSendMailConfermaEmail;

        event SendMail OnSendMailRecoveryPassword;

        event NotifyUser OnNotifyUser;
        
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

        Task CreateUser(string username, string email, string nome, string cognome, Roles role);
        
        Task ConfirmEmail(string username, string code);

        Task<string> GetUserRole(string userId);
        
        Task<SediinIdentityUserWithRoles> GetUserByUsername(string username);
        
        Task<SediinIdentityUserWithRoles> GetUserById(string id);
        
        Task ChangePassword(string username, string currentPassword, string newPassword);

        Task<(IList<SediinIdentityUserWithRoles> Users, int TotalCount)> GetUsersPagedAsync(UtentiRicercaVM filtri , int pageNumber, int pageSize);
        
        Task<IList<SediinIdentityUserWithRoles>> GetAllUsersAsync(UtentiRicercaVM filtri);

        Task UpdateUser(SediinIdentityUser_DTO user);

        Task DeleteUserById(string id);

        Task DisableUserById(string id);

        Task EnableUserById(string id);
    }
}
