using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Sediin.Core.WebUi.Areas.Administrator.Models;
using Sediin.Core.WebUi.Areas.Identity.Data;
using Sediin.Core.WebUi.Data;
using Sediin.Core.WebUi.Models;

namespace Sediin.Core.WebUi.Controllers
{
    public class HomeController : ApplicationController
    {
        //private readonly SignInManager<IdentityUser> _signInManager;
        //private readonly ILogger<LoginModel> _logger;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            SignInManager<IdentityUser> signInManager,
            ILogger<HomeController> logger,
            EmailSender emailSender) : base(signInManager, emailSender)
        {
            //_signInManager = signInManager;
            _logger = logger;
            //_emailSender = emailSender;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(Roles.Administrator.ToString()))
                {
                    //List<MenuModel> menuModel = new Menu().GetMenu(Roles.Administrator.ToString());
                    AdministratorModel model = new AdministratorModel();
                    return View("~/Areas/Administrator/Views/Index.cshtml", model);
                }
            }

            return View();
        }

        public IActionResult Privacy()
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
