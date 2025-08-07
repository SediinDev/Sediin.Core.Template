using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sediin.Core.Identity;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Models;
using System.Diagnostics;

namespace Sediin.Core.WebUi.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
           

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
