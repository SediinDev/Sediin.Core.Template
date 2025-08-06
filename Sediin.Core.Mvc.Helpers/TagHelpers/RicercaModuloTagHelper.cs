using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using HtmlAgilityPack;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("ricerca-modulo")]
    [HtmlTargetElement("RicercaModulo")]
    public class RicercaModuloTagHelper : TagHelper
    {
        public string HeaderText { get; set; }
        public string RicercaAction { get; set; }
        public bool ExecuteOnReady { get; set; } = true;
        public bool ShowResetButton { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = output.GetChildContentAsync().Result;
            var html = content.GetContent();

            var sb = new StringBuilder();

            sb.Append("<div class=\"accordion\" id=\"accordionPanelsRicercae\">");
            sb.Append("<div class=\"accordion-item\">");
            sb.Append("<h2 class=\"accordion-header\" id=\"panelRicerca-headingOne\">");
            sb.Append("<button class=\"accordion-button text-dark\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#panelRicerca-collapseOne\" aria-expanded=\"true\" aria-controls=\"panelRicerca-collapseOne\">");
            sb.Append("<h4>" + HeaderText + "</h4>");
            sb.Append("</button>");
            sb.Append("</h2>");
            sb.Append("<div id=\"panelRicerca-collapseOne\" class=\"accordion-collapse collapse show\" aria-labelledby=\"panelRicerca-headingOne\">");
            sb.Append("<div class=\"accordion-body\">");

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            if (ShowResetButton)
            {
                try
                {
                    var buttons = htmlDoc.DocumentNode.Descendants("button")
                        .Where(n => n.Attributes.Any(a => a.Value == "submit"))
                        .ToList();

                    var submitBtn = buttons.FirstOrDefault();
                    if (submitBtn != null)
                    {
                        var resetBtn = htmlDoc.CreateElement("button");
                        resetBtn.Attributes.Add("type", "reset");
                        resetBtn.Attributes.Add("class", "btn btn-danger ml-1");
                        resetBtn.InnerHtml = "Reset modulo";

                        submitBtn.ParentNode?.InsertAfter(resetBtn, submitBtn);
                    }
                }
                catch
                {
                    // gestione silenziosa
                }
            }

            sb.Append(htmlDoc.DocumentNode.OuterHtml);

            sb.Append("</div></div></div></div>"); // chiusure accordion
            sb.Append("<div id=\"resultRicerca\" class=\"mt-3\"></div>");

            // Script
            sb.Append("<script>");
            if (ShowResetButton)
            {
                sb.Append(" $('#accordionPanelsRicercae').find(\"button[type='reset']\").on(\"click\", function () { ");
                sb.Append("$('#accordionPanelsRicercae').find(\"input[type='hidden']\").val(''); ");
                sb.Append("$('#accordionPanelsRicercae').find(\".field-validation-error\").html(''); ");
                sb.Append("});");
            }

            sb.Append($"updateListRicercaFromAction('{RicercaAction}', false);");

            if (ExecuteOnReady)
            {
                sb.Append("$(function() {");
                sb.Append("alertWaid();");
                sb.Append($"updateListRicercaFromAction('{RicercaAction}', true);");
                sb.Append("panelRicercaCollapse(false);");
                sb.Append("});");
            }

            sb.Append(@"
function panelRicercaCollapse(collapse) {
    if (!collapse) {
        $('#panelRicerca-collapseOne').collapse('hide').removeClass('show');
    } else {
        $('#panelRicerca-collapseOne').collapse('show').addClass('show');
    }
}
");
            sb.Append("</script>");

            output.TagName = null; // non vogliamo racchiuderlo in un <ricerca-modulo>
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
