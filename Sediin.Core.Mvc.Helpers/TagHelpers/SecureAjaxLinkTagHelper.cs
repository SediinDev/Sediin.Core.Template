using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sediin.Core.Mvc.Helpers.Security;
using System;
using System.Text.Json;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    //    <a asp-controller="Prodotti" asp-action="Dettaglio"
    //   asp-route-id="123"
    //   asp-route-category="Elettronica"
    //   asp-ajax="true"
    //   asp-ajax-begin="onBegin"
    //   asp-ajax-failure="onFail"
    //   asp-ajax-success="onSuccess">
    //    Vedi prodotto
    //</a>

    //<a asp-controller= "Ordini" asp-action= "Filtra"
    //   asp-model= "@ModelFiltro"
    //   asp-ajax= "true"
    //   asp-ajax-begin= "onBegin"
    //   asp-ajax-failure= "onFail"
    //   asp-ajax-success= "onSuccess" >
    //    Filtra ordini
    //</a>
    [HtmlTargetElement("a", Attributes = "asp-controller, asp-action")]
    public class SecureAjaxLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory _urlHelperFactory;

        public SecureAjaxLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        // Singoli parametri da criptare e passare in query string
        [HtmlAttributeName(DictionaryAttributePrefix = "asp-route-")]
        public Dictionary<string, string> RouteValues { get; set; } = new();

        // Model complesso da serializzare e criptare
        [HtmlAttributeName("asp-model")]
        public object Model { get; set; }

        // AJAX unobtrusive
        [HtmlAttributeName("asp-ajax")]
        public bool Ajax { get; set; } = false;

        [HtmlAttributeName("asp-ajax-begin")]
        public string AjaxBegin { get; set; }

        [HtmlAttributeName("asp-ajax-failure")]
        public string AjaxFailure { get; set; }

        [HtmlAttributeName("asp-ajax-success")]
        public string AjaxSuccess { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);

            string encryptedParam = null;

            if (Model != null)
            {
                var json = JsonSerializer.Serialize(Model);
                encryptedParam = HttpUtility.UrlEncode(CryptoHelper.Encrypt(json));
            }
            else if (RouteValues.Any())
            {
                var queryString = string.Join("&", RouteValues.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
                var encrypted = CryptoHelper.Encrypt(queryString);
                encryptedParam = HttpUtility.UrlEncode(queryString);
            }

            var url = urlHelper.Action(Action, Controller, encryptedParam != null ? new { q = encryptedParam } : null);

            output.TagName = "a";
            output.Attributes.SetAttribute("href", url);

            if (Ajax)
            {
                output.Attributes.SetAttribute("data-ajax", "true");
                if (!string.IsNullOrEmpty(AjaxBegin))
                    output.Attributes.SetAttribute("data-ajax-begin", AjaxBegin);
                if (!string.IsNullOrEmpty(AjaxFailure))
                    output.Attributes.SetAttribute("data-ajax-failure", AjaxFailure);
                if (!string.IsNullOrEmpty(AjaxSuccess))
                    output.Attributes.SetAttribute("data-ajax-success", AjaxSuccess);
            }
        }
    }
}
