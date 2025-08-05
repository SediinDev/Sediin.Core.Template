using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Areas.Backend.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult NavMenu()
        {
            return View();
        }
        public IActionResult SideMenu()
        {
            return View();
        }
    }
}
