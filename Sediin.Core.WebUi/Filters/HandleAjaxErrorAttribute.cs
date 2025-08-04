using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sediin.Core.WebUi.Filters
{
    public class HandleAjaxErrorAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
                return;

            var request = context.HttpContext.Request;
            var isAjax = request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                context.Result = new JsonResult(new
                {
                    success = false,
                    message = context.Exception.Message
                })
                {
                    StatusCode = 500
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
