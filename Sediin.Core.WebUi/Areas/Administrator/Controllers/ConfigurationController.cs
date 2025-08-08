using Microsoft.AspNetCore.Mvc;
using Sediin.Core.TemplateConfiguration.Models;
using Sediin.Core.WebUi.Filters;
using System.Threading.Tasks;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    [AuthorizeSediin(Roles = [Identity.Roles.SuperAdmin])]
    public class ConfigurationController : BaseController
    {
        public IActionResult RagioneSociale()
        {
            var model = _sediinConfiguration.Get().Result.RagioneSociale;
            return AjaxView(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> RagioneSociale(RagioneSociale model)
        {
            await _sediinConfiguration.SaveRagioneSociale(model);
            return Content("Ragione sociale aggiornata");
        }

        public IActionResult EmailSettings()
        {
            var model = _sediinConfiguration.Get().Result.EmailSettings;
            return AjaxView(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailSettings(EmailSettings model)
        {
            await _sediinConfiguration.SaveEmailSettings(model);
            return Content("Email settings aggiornati");
        }
    }
}
