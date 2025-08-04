using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Sediin.Core.WebUi.Filters
{
    public class RedirectIfNotAjaxAttribute : ActionFilterAttribute
    {
        public string Url { get; set; } = "/Error";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (!isAjax)
            {
                // Costruisce l'URL relativo se necessario
                var redirectUrl = Url.StartsWith("/") ? Url : "/" + Url;

                context.Result = new RedirectResult(redirectUrl);
            }

            base.OnActionExecuting(context);
        }
    }
}
