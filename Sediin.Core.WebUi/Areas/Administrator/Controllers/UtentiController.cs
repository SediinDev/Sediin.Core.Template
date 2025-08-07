using Microsoft.AspNetCore.Mvc;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;
using Sediin.Core.Mvc.Helpers.PagingHelpers;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Filters;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    [AuthorizeSediin]
    public class UtentiController : BaseController
    {
        [AuthorizeSediin(Roles = new[] { Identity.Roles.Administrator })]
        public IActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [AuthorizeSediin(Roles = new[] { Identity.Roles.Administrator })]
        public async Task<IActionResult> Ricerca(UtentiRicercaVM filtri, int? page = 1)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetUsersPagedAsync(filtri, page.GetValueOrDefault(), 10);

            var resultModel = PagingHelper.GetModelWithPaging<UtentiVM, SediinIdentityUser>(page, _result.Users, filtri, _result.TotalCount, 10);

            return AjaxView("RicercaList", resultModel);

        }

        [AuthorizeSediin(Roles = new[] { Identity.Roles.Administrator })]
        public async Task<IActionResult> RicercaExcel(UtentiRicercaVM filtri)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetAllUsersAsync(filtri);
            return ExportExcel(_result, "RicercaList");
        }

        [RedirectIfNotAjax]
        public async Task<IActionResult> ChangePassword() 
        {
            return AjaxView();
        }

        [HttpPost]
        [RedirectIfNotAjax]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model) 
        {
            await _unitOfWorkIdentity.AuthService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
            return Content("Password cambiata");
        }
    }
}
