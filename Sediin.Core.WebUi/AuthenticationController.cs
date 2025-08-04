using Microsoft.AspNetCore.Mvc;
using Sediin.Core.WebUi.Controllers;
using Sediin.Core.WebUi.Models;

namespace Sediin.Core.WebUi
{

    public class AuthenticationController : BaseController
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Authentication model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Dati non validi." });
            }

            var result = await _unitOfWorkIdentity.AuthService.LoginAsync(model.Username, model.Password, model.RememberMe);

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Credenziali errate." });
            }
        }
    }
}
