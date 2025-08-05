using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sediin.Core.DataAccess.Entities;

namespace Sediin.Core.WebUi.Areas.Backend.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public async Task< IActionResult> Index()
        {
            return AjaxView();
        }
    }
}
