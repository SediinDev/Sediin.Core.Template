using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("ajax-form")]
    public class AjaxFormTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public AjaxFormTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-route")]
        public object RouteValues { get; set; }

        [HtmlAttributeName("method")]
        public string Method { get; set; } = "POST";

        [HtmlAttributeName("update-target-id")]
        public string UpdateTargetId { get; set; }

        [HtmlAttributeName("ajax-mode")]
        public string AjaxMode { get; set; } = "replace";

        [HtmlAttributeName("on-success")]
        public string OnSuccess { get; set; }

        [HtmlAttributeName("on-failure")]
        public string OnFailure { get; set; }

        [HtmlAttributeName("on-begin")]
        public string OnBegin { get; set; }

        [HtmlAttributeName("on-complete")]
        public string OnComplete { get; set; }

        [HtmlAttributeName("loading-element-id")]
        public string LoadingElementId { get; set; }

        [HtmlAttributeName("confirm")]
        public string Confirm { get; set; }

        [HtmlAttributeName("include-antiforgery-token")]
        public bool IncludeAntiforgeryToken { get; set; } = true;

        [HtmlAttributeName("html-attributes")]
        public IDictionary<string, string> HtmlAttributes { get; set; } = new Dictionary<string, string>();

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;

            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            var url = urlHelper.Action(Action, Controller, RouteValues);

            output.Attributes.SetAttribute("method", Method);
            if (!string.IsNullOrEmpty(url))
                output.Attributes.SetAttribute("action", url);

            output.Attributes.SetAttribute("data-ajax", "true");
            output.Attributes.SetAttribute("data-ajax-method", Method.ToUpper());

            if (!string.IsNullOrEmpty(UpdateTargetId))
                output.Attributes.SetAttribute("data-ajax-update", $"#{UpdateTargetId}");

            if (!string.IsNullOrEmpty(AjaxMode))
                output.Attributes.SetAttribute("data-ajax-mode", AjaxMode);

            if (!string.IsNullOrEmpty(OnSuccess))
                output.Attributes.SetAttribute("data-ajax-success", OnSuccess);

            if (!string.IsNullOrEmpty(OnFailure))
                output.Attributes.SetAttribute("data-ajax-failure", OnFailure);

            if (!string.IsNullOrEmpty(OnBegin))
                output.Attributes.SetAttribute("data-ajax-begin", OnBegin);

            if (!string.IsNullOrEmpty(OnComplete))
                output.Attributes.SetAttribute("data-ajax-complete", OnComplete);

            if (!string.IsNullOrEmpty(LoadingElementId))
                output.Attributes.SetAttribute("data-ajax-loading", $"#{LoadingElementId}");

            if (!string.IsNullOrEmpty(Confirm))
                output.Attributes.SetAttribute("data-ajax-confirm", Confirm);

            if (HtmlAttributes != null)
            {
                foreach (var attr in HtmlAttributes)
                {
                    output.Attributes.SetAttribute(attr.Key, attr.Value);
                }
            }

            var childContent = await output.GetChildContentAsync();
            output.Content.SetHtmlContent(childContent);

            if (IncludeAntiforgeryToken)
            {
                var antiforgery = ViewContext.HttpContext.RequestServices.GetService<Microsoft.AspNetCore.Antiforgery.IAntiforgery>();
                var tokens = antiforgery.GetAndStoreTokens(ViewContext.HttpContext);
                var tokenTag = new TagBuilder("input");
                tokenTag.Attributes.Add("type", "hidden");
                tokenTag.Attributes.Add("name", tokens.FormFieldName);
                tokenTag.Attributes.Add("value", tokens.RequestToken);
                output.PostContent.AppendHtml(tokenTag);
            }
        }
    }
}
