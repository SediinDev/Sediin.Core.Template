using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Areas.Administrator.Controllers
{
    public class UtentiController : BaseController
    {
        public IActionResult Ricerca()
        {
            return AjaxView();
        }
    
    }
}
