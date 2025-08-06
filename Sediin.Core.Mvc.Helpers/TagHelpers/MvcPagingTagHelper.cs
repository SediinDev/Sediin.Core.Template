using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("mvc-paging")]
    public class MvcPagingTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public MvcPagingTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        /// <summary>Nome azione MVC</summary>
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        /// <summary>Nome controller MVC</summary>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>Parametri route aggiuntivi</summary>
        [HtmlAttributeName("asp-route")]
        public object RouteValues { get; set; }

        /// <summary>Dimensione pagina</summary>
        [HtmlAttributeName("page-size")]
        public int PageSize { get; set; } = 10;

        /// <summary>Pagina corrente</summary>
        [HtmlAttributeName("page-index")]
        public int PageIndex { get; set; } = 1;

        /// <summary>Numero totale righe</summary>
        [HtmlAttributeName("total-rows")]
        public int TotalRows { get; set; } = 10;

        /// <summary>Id elemento da aggiornare con ajax</summary>
        [HtmlAttributeName("update-target-id")]
        public string UpdateTargetId { get; set; }

        /// <summary>Metodo HTTP ajax (GET o POST)</summary>
        [HtmlAttributeName("ajax-method")]
        public string AjaxMethod { get; set; } = "GET";

        /// <summary>Javascript chiamato all’inizio ajax</summary>
        [HtmlAttributeName("on-begin")]
        public string OnBegin { get; set; }

        /// <summary>Javascript chiamato in caso di successo ajax</summary>
        [HtmlAttributeName("on-success")]
        public string OnSuccess { get; set; }

        /// <summary>Javascript chiamato in caso di fallimento ajax</summary>
        [HtmlAttributeName("on-failure")]
        public string OnFailure { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        private IUrlHelper UrlHelper => _urlHelperFactory.GetUrlHelper(ViewContext);

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.SetAttribute("aria-label", "pagination-navigation");

            var totalPages = (int)Math.Ceiling((double)TotalRows / PageSize);
            if (totalPages == 0) totalPages = 1;
            var currentStep = (int)Math.Ceiling((double)PageIndex / 10);
            var start = (currentStep - 1) * 10 + 1;
            var end = Math.Min(start + 10 - 1, totalPages);

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            // Helper per costruire URL con pagina e parametri aggiuntivi
            string BuildPageUrl(int page)
            {
                var routeDict = RouteValues == null
                    ? new Microsoft.AspNetCore.Routing.RouteValueDictionary()
                    : new Microsoft.AspNetCore.Routing.RouteValueDictionary(RouteValues);

                routeDict["page"] = page;
                return UrlHelper.Action(Action, Controller, routeDict);
            }

            // Funzione per costruire attributi ajax data-*
            Dictionary<string, string> BuildAjaxAttributes()
            {
                var dict = new Dictionary<string, string>();
                dict["data-ajax"] = "true";
                dict["data-ajax-method"] = AjaxMethod.ToUpper();

                if (!string.IsNullOrEmpty(UpdateTargetId))
                    dict["data-ajax-update"] = $"#{UpdateTargetId}";

                if (!string.IsNullOrEmpty(OnBegin))
                    dict["data-ajax-begin"] = OnBegin;

                if (!string.IsNullOrEmpty(OnSuccess))
                    dict["data-ajax-success"] = OnSuccess;

                if (!string.IsNullOrEmpty(OnFailure))
                    dict["data-ajax-failure"] = OnFailure;

                dict["data-ajax-mode"] = "replace";

                return dict;
            }

            Dictionary<string, string> ajaxAttrs = BuildAjaxAttributes();

            // Funzione per creare singolo <li><a></a></li>
            TagBuilder CreatePageItem(string text, int? page, bool disabled = false, bool active = false, string ariaLabel = null)
            {
                var li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (disabled) li.AddCssClass("disabled");
                if (active) li.AddCssClass("active");

                var a = new TagBuilder("a");
                a.AddCssClass("page-link");
                if (ariaLabel != null)
                    a.Attributes["aria-label"] = ariaLabel;

                if (page.HasValue && !disabled && !active)
                {
                    a.Attributes["href"] = BuildPageUrl(page.Value);
                    foreach (var attr in ajaxAttrs)
                        a.Attributes[attr.Key] = attr.Value;
                }
                else
                {
                    a.Attributes["href"] = "#";
                    a.Attributes["onclick"] = "return false;";
                    a.Attributes["style"] = "cursor:default;";
                }

                a.InnerHtml.AppendHtml(text);
                li.InnerHtml.AppendHtml(a);
                return li;
            }

            // Precedente
            ul.InnerHtml.AppendHtml(CreatePageItem("&laquo;", start - 1, disabled: start == 1, ariaLabel: "Precedente"));

            // Pagine numerate
            for (int i = start; i <= end; i++)
            {
                ul.InnerHtml.AppendHtml(CreatePageItem(i.ToString(), i, active: i == PageIndex));
            }

            // Successiva
            ul.InnerHtml.AppendHtml(CreatePageItem("&raquo;", end + 1, disabled: end == totalPages, ariaLabel: "Successiva"));

            // Costruisco html finale
            output.Content.SetHtmlContent(ul);

            await Task.CompletedTask;
        }
    }
}
