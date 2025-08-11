using Microsoft.AspNetCore.Mvc;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Entities.DTO;
using Sediin.Core.Identity.Models;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Filters;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    [AuthorizeSediin]
    public class UtentiController : BaseController
    {
        [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin, Identity.Roles.Admin])]
        public IActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin, Identity.Roles.Admin])]
        public async Task<IActionResult> Ricerca(UtentiRicercaVM filtri, int? page = 1)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetUsersPagedAsync(filtri, page.GetValueOrDefault(), 10);

            var resultModel = GetModelWithPaging<UtentiVM, SediinIdentityUserWithRoles>(page, _result.Users, filtri, _result.TotalCount, 10);

            return AjaxView("RicercaList", resultModel);
        }


        [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin, Identity.Roles.Admin])]
        public async Task<IActionResult> RicercaExcel(UtentiRicercaVM filtri)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetAllUsersAsync(filtri);
            return ExportExcel(_result, "RicercaList");
        }

        [RedirectIfNotAjax]
        [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin, Identity.Roles.Admin])]
        public async Task<IActionResult> ModificaUtente(string id)
        {
            var user = await _unitOfWorkIdentity.AuthService.GetUserById(id);

            var model = new SediinIdentityUser_DTO()
            {
                Cognome = user.User.Cognome,
                ConfirmEmail = user.User.Email,
                Email = user.User.Email,
                Id = user.User.Id,
                Nome = user.User.Nome,
                Ruolo = user.Roles.FirstOrDefault(),
                Username = user.User.UserName,

            };// _autoMapper.Map<SediinIdentityUser_DTO>(user);

            return AjaxView(model: model);
        }

        [HttpPost]
        [RedirectIfNotAjax]
        [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin, Identity.Roles.Admin])]
        public async Task<IActionResult> ModificaUtente(SediinIdentityUser_DTO model)
        {
            await _unitOfWorkIdentity.AuthService.UpdateUser(model);
            return Content("Utente aggiornato");
        }

        /// <summary>
        /// per tutti utenti loggate
        /// </summary>
        /// <returns></returns>
        [RedirectIfNotAjax]
        public async Task<IActionResult> ChangePassword()
        {
            return AjaxView();
        }

        /// <summary>
        /// per tutti utenti loggate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [RedirectIfNotAjax]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            await _unitOfWorkIdentity.AuthService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            return Content("Password cambiata");
        }
    }
}
