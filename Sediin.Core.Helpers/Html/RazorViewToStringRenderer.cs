using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace Sediin.Core.Helpers.Html
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync(string viewName, object model);
    }

    public class RazorViewToStringRenderer : IRazorViewToStringRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewToStringRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            viewName = "~/Areas/TemplateEmail/Views/" + viewName.Replace(".cshtml", "") + ".cshtml";

            var actionContext = new ActionContext(
                new DefaultHttpContext { RequestServices = _serviceProvider },
                new RouteData(),
                new ActionDescriptor());

            var viewResult = _viewEngine.GetView(null, viewName, true);

            if (viewResult.View == null)
            {
                throw new ArgumentNullException($"View '{viewName}' non trovata.");
            }

            using var sw = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                viewResult.View,
                new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary()) { Model = model },
                new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
                sw,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return sw.ToString();
        }
    }

}
