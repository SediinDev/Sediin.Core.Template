using Microsoft.AspNetCore.Mvc;
using Sediin.Core.WebUi.Filters;
using Sediin.Core.WebUi.Models;
using System.Collections.Specialized;
using System.Web;

namespace Sediin.Core.WebUi.Controllers
{
    public class AuthenticationController : BaseController
    {
        public IActionResult Login()
        {
            return AjaxView();
        }

        [HttpPost]
        [RedirectIfNotAjax(Url = "/")]
        public async Task<IActionResult> Login(Authentication model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "I dati inseriti non sono validi." });
            }

            var result = await _unitOfWorkIdentity.AuthService.LoginAsync(model.Username, model.Password, model.RememberMe);

            if (result.Succeeded)
            {
                return Json(new { success = true });
            }

            if (result.RequiresTwoFactor)
            {
                return Json(new { success = false });
                //return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }

            if (result.IsLockedOut)
            {
                throw new Exception("L'account utente è bloccato.");
            }
            else
            {
                throw new Exception("Tentativo di accesso non valido.");
            }
        }

        [RedirectIfNotAjax(Url = "/")]
        public IActionResult ForgotPassword()
        {
            return AjaxView();
        }

        [HttpPost]
        [RedirectIfNotAjax(Url = "/")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                _unitOfWorkIdentity.AuthService.OnSendMailConfermaEmail += AuthService_OnSendMailConfermaEmail;

                _unitOfWorkIdentity.AuthService.OnSendMailRecoveryPassword += AuthService_OnSendMailRecoveryPassword;

                await _unitOfWorkIdentity.AuthService.RecoveryPassword(model.Email);

                return Content("Le è stata inviata un'email per confermare la sua richiesta.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task AuthService_OnSendMailRecoveryPassword(string email, string userId, string token)
        {
            string _url = CreateUrl("RecoveryPassword", userId, token);
            string subject = "Cambio password";
            string body = "";
            await _emailSender.SendEmailAsync(email, subject, body);
        }

        private async Task AuthService_OnSendMailConfermaEmail(string email, string userId, string token)
        {
            string _url = CreateUrl("ConfirmEmail", userId, token);
            string subject = "";
            string body = "Conferma account";
            await _emailSender.SendEmailAsync(email, subject, body);
        }

        private string CreateUrl(string action, string userId, string token)
        {
            NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
            c.Add("userId", userId);
            c.Add("code", token);

            return $"{_configuration["UrlPortale"]}/{Url.Action(action, "Authentication")}?{c.ToString()}";
        }
    }
}
