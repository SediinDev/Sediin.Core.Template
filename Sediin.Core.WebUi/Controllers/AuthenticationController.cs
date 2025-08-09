using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Sediin.Core.WebUi.Areas.TemplateEmail.Models;
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
                return Json(new { success = true, message = "Utente autenticato. Redirect in corso..." });
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

        public async Task<IActionResult> LogOff()
        {
            await _unitOfWorkIdentity.AuthService.LogoutAsync();

            return Redirect("/");
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


        [AllowAnonymous]
        public IActionResult ResetPassword(string code)
        {
            return code == null ? AjaxView("Error") : AjaxView();
        }

        [HttpPost]
        //[RedirectIsNotAjax]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                await _unitOfWorkIdentity.AuthService.ResetPassword(model.Username, model.Code, model.Password);

                return Content("Nuova Password e stata reimpostata, attendere verra reindirizzato alla login...");
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [AllowAnonymous]
        public IActionResult Register()
        {
            return AjaxView();
        }

        [HttpPost]
        //[RedirectIsNotAjax]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                _unitOfWorkIdentity.AuthService.OnSendMailConfermaEmail += AuthService_OnSendMailConfermaEmail;

                await _unitOfWorkIdentity.AuthService.CreateUser(model.UserName, model.Email, model.Nome, model.Cognome, Identity.Roles.SuperAdmin);

                return Json(new
                {
                    success = true,
                    message = "Utente registrato correttamente. Controlla la tua email per confermare l'indirizzo."
                });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            try
            {
                await _unitOfWorkIdentity.AuthService.ConfirmEmail(userId, code);

                return AjaxView(model: new { isValid = true });
            }
            catch (Exception ex)
            {
                return AjaxView(model: new { isValid = false });
            }
        }

        #region helper
        private async Task AuthService_OnSendMailRecoveryPassword(string email, string userId, string token, string nome, string cognome)
        {
            string _url = CreateUrl("ResetPassword", userId, token);
            string subject = "Cambio password";

            string htmlBody = await RenderViewToStringAsync(
                    "Authentication/ResetPassword", new ResetPasswordModel
                    {
                        Url = _url,
                        Nome = nome,
                        Cognome = cognome,
                        Email = email,
                    });

            await _emailSender.SendEmailAsync(email, subject, htmlBody);
        }

        private async Task AuthService_OnSendMailConfermaEmail(string email, string userId, string token, string nome, string cognome)
        {
            string _url = CreateUrl("ConfirmEmail", userId, token);
            string subject = "Conferma email";

            string htmlBody = await RenderViewToStringAsync(
                   "Authentication/ConfirmEmail", new ConfirmEmailModel
                   {
                       Url = _url,
                       Cognome = cognome,
                       Nome = nome,
                       Email = email,
                   });

            await _emailSender.SendEmailAsync(email, subject, htmlBody);
        }

        private string CreateUrl(string action, string userId, string token)
        {
            NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
            c.Add("userId", userId);
            c.Add("code", token);

            var _url = _configuration["UrlPortale"]?.TrimEnd('/');

            _url = _url + $"{Url.Action(action, "Authentication")}?{c.ToString()}";
            return _url;
        }
        #endregion

    }
}
