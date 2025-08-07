using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sediin.Core.WebUi.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeSediinAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Identity.Roles[] Roles { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var headers = context.HttpContext.Request.Headers;

            bool isAjax = headers.ContainsKey("X-Requested-With") && headers["X-Requested-With"] == "XMLHttpRequest";

            // 🔐 Non autenticato
            if (user == null || !user.Identity?.IsAuthenticated == true)
            {
                if (isAjax)
                {
                    context.Result = new JsonResult(new
                    {
                        success = true,
                        message = "Autenticazione richiesta. Effettua il login."
                    })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                    return;
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Authentication", null);
                    return;
                }
            }

            // 🎟️ Nessun ruolo richiesto → accesso permesso
            if (Roles == null || Roles.Length == 0)
            {
                return;
            }

            // ✅ Verifica se utente ha almeno un ruolo richiesto
            bool isInRole = Roles.Any(role => user.IsInRole(role.ToString()));

            if (isInRole)
                return;

            // ❌ Non autorizzato
            if (isAjax)
            {
                context.Result = new JsonResult(new
                {
                    success = true,
                    message = "Non sei autorizzato ad accedere a questa risorsa."
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            // 🖼️ Risposta HTML: carica view AccessDenied da /Areas/Backend/Views/Shared/
            var viewEngine = context.HttpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;
            var tempDataProvider = context.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            var actionContext = new ActionContext(context.HttpContext, context.RouteData, new ActionDescriptor());

            using var sw = new StringWriter();
            var viewResult = viewEngine.GetView(
                executingFilePath: null,
                viewPath: "/Areas/Backend/Views/Shared/AccessDenied.cshtml",
                isMainPage: true);

            if (!viewResult.Success)
            {
                // fallback: messaggio plain text
                context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.HttpContext.Response.ContentType = "text/plain";
                await context.HttpContext.Response.WriteAsync("Accesso negato: non sei autorizzato.");
                context.Result = new EmptyResult();
                return;
            }

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                ["Message"] = "Non sei autorizzato ad accedere a questa risorsa."
            };

            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                viewData,
                new TempDataDictionary(context.HttpContext, tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);

            context.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.HttpContext.Response.ContentType = "text/html";
            await context.HttpContext.Response.WriteAsync(sw.ToString());

            context.Result = new EmptyResult();
        }
    }
}
