using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.Helpers.Html;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Entities;
using Sediin.Core.TemplateConfiguration;
using System.Text;

public class BaseController : Controller
{
    protected ILogger<BaseController> _logger;
    protected IUnitOfWorkIdentity _unitOfWorkIdentity;
    protected IUnitOfWorkDataAccess _unitOfWorkDataAccess;
    protected IEmailSender _emailSender;
    protected IConfiguration _configuration;
    protected IRazorViewToStringRenderer _razorViewRenderer;
    protected IMapper _autoMapper;
    protected IBaseConfiguration _baseConfiguration;

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

        if (_unitOfWorkDataAccess == null)
            _unitOfWorkDataAccess = HttpContext.RequestServices.GetService<IUnitOfWorkDataAccess>();

        if (_emailSender == null)
            _emailSender = HttpContext.RequestServices.GetService<IEmailSender>();

        if (_configuration == null)
            _configuration = HttpContext.RequestServices.GetService<IConfiguration>();

        if (_razorViewRenderer == null)
            _razorViewRenderer = HttpContext.RequestServices.GetService<IRazorViewToStringRenderer>();

        if (_autoMapper == null)
            _autoMapper = HttpContext.RequestServices.GetService<IMapper>();

        if (_baseConfiguration == null)
            _baseConfiguration = HttpContext.RequestServices.GetService<IBaseConfiguration>();

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

    internal string ModelStateErrorToString(ModelStateDictionary modelState)
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

    internal IActionResult ExportExcel<T>(IEnumerable<T> model, string fileName)
    {
        if (model == null || model?.ToList().Count == 0)
            throw new Exception("Nessun record trovato");

        var content = Sediin.Core.Mvc.Helpers.ExcelHelper.Excel.CreateExcelFromList(model);

        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
    }

    public async Task<SediinIdentityUser> GetUser()
    {
        return await _unitOfWorkIdentity.AuthService.GetUserByUsername(User.Identity.Name);
    }
}
