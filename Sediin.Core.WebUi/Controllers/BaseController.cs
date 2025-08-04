using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sediin.Core.Identity.Abstract;

public abstract class BaseController : Controller
{
    protected ILogger<BaseController> _logger;
    protected IUnitOfWorkIdentity _unitOfWorkIdentity;
    //protected IEmailSender _emailSender;

    #pragma warning disable
    public BaseController()
    {
    }

    #pragma warning disable
    // Metodo eseguito prima di ogni azione, qui risolvi i servizi
    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        if (_logger == null)
            _logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
        if (_unitOfWorkIdentity == null)
            _unitOfWorkIdentity = HttpContext.RequestServices.GetService<IUnitOfWorkIdentity>();
        //if (_emailSender == null)
        //    _emailSender = HttpContext.RequestServices.GetService<IEmailSender>();

        base.OnActionExecuting(context);
    }

    public IActionResult AjaxView(string viewName = null, object model = null)
    {
        try
        {
            bool isAjax = HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Se non è specificato il nome della view, usa il nome dell'azione corrente
            viewName ??= ControllerContext.ActionDescriptor.ActionName;

            return isAjax
                ? PartialView(viewName, model)
                : View(viewName, model);
        }
        catch
        {
            viewName ??= ControllerContext.ActionDescriptor.ActionName;
            return View(viewName, model);
        }
    }
}
