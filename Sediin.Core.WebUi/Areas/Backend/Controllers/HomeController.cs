using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Areas.Backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
