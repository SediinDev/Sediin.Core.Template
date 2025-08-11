using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Sediin.Core.TemplateConfiguration.Models;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Filters;
using System.Configuration;
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

        public async Task<IActionResult> EmailTest()
        {
            var model = new TestMailSettingConfigModel();
            return AjaxView(model: model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailTest(TestMailSettingConfigModel model)
        {
            if(model == null)
                return Content("Email Test non valorizzata!");
            if (model.EmailTo == null)
                return Content("Email Test non valorizzata!");
            if (model.Oggetto == null)
                model.Oggetto = "";
            if (model.Messaggio == null)
                model.Messaggio = "";

            var message = new SimpleMailMessage
            {
                Body = model.Messaggio,
                Subject = model.Oggetto,
                ToEmail = model.EmailTo
            };

            await _emailSender.SendEmailAsync(message.ToEmail, message.Subject, message.Body);

            return Content("Email Test inviata a " + model.EmailTo);
        }
    }
}
