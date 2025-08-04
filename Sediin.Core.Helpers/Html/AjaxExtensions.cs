using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Sediin.Core.Helpers.Html
{
    #pragma warning disable
    public static class AjaxExtensions
    {
        public static IDisposable BeginAjaxForm(this IHtmlHelper htmlHelper,
            string action,
            string controller,
            object routeValues = null,
            string method = "POST",
            string updateTargetId = "",
            string ajaxMode = "replace",
            string onSuccess = null,
            string onFailure = null,
            string onBegin = null,
            string onComplete = null,
            string loadingElementId = null,
            string confirm = null,
            object htmlAttributes = null,
            bool includeAntiforgeryToken = true)
        {
            var formTag = new TagBuilder("form")
            {
                TagRenderMode = TagRenderMode.StartTag
            };

            formTag.Attributes["method"] = method;
            formTag.Attributes["data-ajax"] = "true";
            formTag.Attributes["data-ajax-method"] = method.ToUpper();

            // ✅ Usa IUrlHelperFactory per ottenere l’URL corretto
            var urlHelperFactory = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = urlHelperFactory.GetUrlHelper(htmlHelper.ViewContext);
            var url = urlHelper.Action(action, controller, routeValues);
            if (!string.IsNullOrEmpty(url))
                formTag.Attributes["action"] = url;

            if (!string.IsNullOrEmpty(updateTargetId))
                formTag.Attributes["data-ajax-update"] = $"#{updateTargetId}";

            if (!string.IsNullOrEmpty(ajaxMode))
                formTag.Attributes["data-ajax-mode"] = ajaxMode;

            if (!string.IsNullOrEmpty(onSuccess))
                formTag.Attributes["data-ajax-success"] = onSuccess;

            if (!string.IsNullOrEmpty(onFailure))
                formTag.Attributes["data-ajax-failure"] = onFailure;

            if (!string.IsNullOrEmpty(onBegin))
                formTag.Attributes["data-ajax-begin"] = onBegin;

            if (!string.IsNullOrEmpty(onComplete))
                formTag.Attributes["data-ajax-complete"] = onComplete;

            if (!string.IsNullOrEmpty(loadingElementId))
                formTag.Attributes["data-ajax-loading"] = $"#{loadingElementId}";

            if (!string.IsNullOrEmpty(confirm))
                formTag.Attributes["data-ajax-confirm"] = confirm;

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            foreach (var attr in attrs)
                formTag.MergeAttribute(attr.Key, attr.Value?.ToString(), replaceExisting: true);

            htmlHelper.ViewContext.Writer.Write(formTag.RenderStartTag());

            if (includeAntiforgeryToken)
            {
                htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            }

            return new AjaxFormDisposer(htmlHelper);
        }

        private class AjaxFormDisposer : IDisposable
        {
            private readonly IHtmlHelper _htmlHelper;
            public AjaxFormDisposer(IHtmlHelper htmlHelper) => _htmlHelper = htmlHelper;

            public void Dispose()
            {
                _htmlHelper.ViewContext.Writer.Write("</form>");
            }
        }

        private static string ToHtmlString(this IHtmlContent content)
        {
            using var writer = new System.IO.StringWriter();
            content.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
            return writer.ToString();
        }
    }
}
