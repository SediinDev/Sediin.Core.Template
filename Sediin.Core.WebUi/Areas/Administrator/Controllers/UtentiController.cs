using Microsoft.AspNetCore.Mvc;
using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;
using Sediin.Core.Mvc.Helpers.PagingHelpers;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    public class UtentiController : BaseController
    {
        public IActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public async Task<IActionResult> Ricerca(UtentiRicercaVM filtri, int? page = 1)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetUsersPagedAsync(filtri, page.GetValueOrDefault(), 10);

            var resultModel = PagingHelper.GetModelWithPaging<UtentiVM, SediinIdentityUser>(page, _result.Users, null, _result.TotalCount, 10);

            return AjaxView("RicercaList", resultModel);

        }

        public async Task<IActionResult> RicercaExcel(UtentiRicercaVM filtri)
        {
            var _result = await _unitOfWorkIdentity.AuthService.GetAllUsersAsync(filtri);
            return ExportExcel(_result, "RicercaList");
        }

    }
}
