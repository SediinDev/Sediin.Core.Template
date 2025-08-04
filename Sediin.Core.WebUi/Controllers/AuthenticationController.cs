using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sediin.Core.WebUi.Models;
using System.Collections.Specialized;
using System.Web;

namespace Sediin.Core.WebUi.Controllers
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
                return Json(new { success = false, message = "I dati inseriti non sono validi." });
            }

            var result = await _unitOfWorkIdentity.AuthService.LoginAsync(model.Username, model.Password, model.RememberMe);

            if (result.Succeeded)
            {
                return Json(new { success = true/*, returnUrl = model.ReturnUrl*/ });
            }


            if (result.RequiresTwoFactor)
            {
                return Json(new { success = false  });

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

        //[RedirectIsNotAjax]
        public IActionResult ForgotPassword()
        {
            return AjaxView();
        }
       
        [HttpPost]
        //[RedirectIsNotAjax]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                return Content("OK");
                //if (!ModelState.IsValid)
                //{
                //    throw new Exception(ModelStateErrorToString(ModelState));
                //}

                //var user = await UserManager.FindByEmailAsync(model.Email);

                //if (user == null)
                //{
                //    throw new Exception("Indirizzo email non trovato");
                //}

                //if (user.LockoutEndDateUtc != null)
                //{
                //    throw new Exception("Utente bloccato, non è possibile recuperare la password");
                //}

                //await IsEmailConfirmed(user);

                //// Inviare un messaggio di posta elettronica con questo collegamento
                //string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                ////var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                //NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                //c.Add("userId", user.Id);
                //c.Add("code", code);

                //var callbackUrl = $"{UriPortale("ResetPassword", "Account")}?{c.ToString()}";

                //RecuperoPasswordConfermaModel _resultModel = new RecuperoPasswordConfermaModel
                //{
                //    UrlConferma = callbackUrl,
                //    Email = user.Email,
                //    Cognome = user.Cognome,
                //    Nome = user.Nome,
                //    Username = user.UserName
                //};

                //await UserManager.SendEmailAsync(user.Id, "Recupero Password", RenderTemplate("Account/ForgotPassword_Mail", _resultModel));

                //var _html = RenderTemplate("Account/ForgotPassword", _resultModel);

                //return Content(_html);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
