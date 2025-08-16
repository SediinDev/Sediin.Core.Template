using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Areas.Backend.Controllers
{
    public class WizardController : BaseController
    {
        public IActionResult Index()
        {
            return AjaxView();
        }
        public IActionResult Step1()
        {
            Thread.Sleep(3000);
            return AjaxView();
        }
        public IActionResult Step2()
        {
            return AjaxView();
        }
        public IActionResult Step3()
        {
            //Thread.Sleep(3000);

            return AjaxView(model:new Sediin.Core.Identity.Entities.DTO.SediinIdentityUser_DTO { });
        }
        public IActionResult Step4()
        {
            return AjaxView();
        }
        public IActionResult Step5()
        {
            throw new Exception("errorrrr");
            return AjaxView();
        }
    }
}
