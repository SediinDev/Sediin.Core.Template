using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sediin.Core.Identity.Abstract;
using System.Text;

public abstract class BaseController : Controller
{
    protected ILogger<BaseController> _logger;
    protected IUnitOfWorkIdentity _unitOfWorkIdentity;
    protected IEmailSender _emailSender;
    protected IConfiguration _configuration;

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
        
        if (_emailSender == null)
            _emailSender = HttpContext.RequestServices.GetService<IEmailSender>();
        
        if (_configuration == null)
            _configuration = HttpContext.RequestServices.GetService<IConfiguration>();

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

    public string ModelStateErrorToString(ModelStateDictionary modelState)
    {
        try
        {
            var sb = new StringBuilder();

            foreach (var entry in modelState)
            {
                foreach (var error in entry.Value.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                        sb.AppendLine($"{error.ErrorMessage}<br/>");
                }
            }

            if (sb.Length == 0)
                sb.Append("Si è verificato un errore!");

            return sb.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }
}
