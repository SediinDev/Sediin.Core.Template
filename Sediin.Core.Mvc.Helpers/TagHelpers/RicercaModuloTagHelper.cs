using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("ricerca-modulo")]
    public class RicercaModuloTagHelper : TagHelper
    {
        public string HeaderText { get; set; } = string.Empty;
        public string RicercaAction { get; set; } = string.Empty;
        public bool ExecuteRicercaOnReady { get; set; } = true;
        public bool ResetButton { get; set; } = true;
        public IHtmlContent? PartialHtml { get; set; }

        /// <summary>
        /// ID del contenitore dei risultati. Default: "resultRicerca"
        /// </summary>
        public string ResultContainerId { get; set; } = "resultRicerca";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var sb = new StringBuilder();

            sb.Append("<div class=\"accordion shadow\" id=\"accordionPanelsRicercae\">");
            sb.Append("<div class=\"accordion-item\">");

            bool hasPartial = PartialHtml != null;

            sb.Append("<h2 class=\"accordion-header\" id=\"panelRicerca-headingOne\">");

            if (hasPartial)
            {
                sb.Append("<button class=\"accordion-button text-dark\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#panelRicerca-collapseOne\" aria-expanded=\"true\" aria-controls=\"panelRicerca-collapseOne\">");
                sb.Append($"<h4 class=\"m-0\">{HeaderText}</h4>");
                sb.Append("</button>");
            }
            else
            {
                sb.Append($"<h4 class=\"m-3\">{HeaderText}</h4>");
            }

            sb.Append("</h2>");

            if (hasPartial)
            {
                sb.Append("<div id=\"panelRicerca-collapseOne\" class=\"accordion-collapse collapse show\" aria-labelledby=\"panelRicerca-headingOne\">");
                sb.Append("<div class=\"accordion-body\">");

                var writer = new System.IO.StringWriter();
                PartialHtml!.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                var html = writer.ToString();

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                try
                {
                    var submitBtn = doc.DocumentNode.Descendants("button")
                        .FirstOrDefault(btn => btn.Attributes["type"]?.Value == "submit");

                    if (submitBtn != null && ResetButton)
                    {
                        var resetButton = doc.CreateElement("button");
                        resetButton.Attributes.Add("type", "reset");
                        resetButton.Attributes.Add("class", "btn btn-danger ms-2");
                        resetButton.InnerHtml = "Reset modulo";

                        submitBtn.ParentNode?.InsertAfter(resetButton, submitBtn);
                    }
                }
                catch
                {
                    // Silenzioso: se fallisce non blocca tutto
                }

                sb.Append(doc.DocumentNode.OuterHtml);
                sb.Append("</div>");
                sb.Append("</div>");
            }

            sb.Append("</div>"); // accordion-item
            sb.Append("</div>"); // accordion

            // ✅ Usa ID dinamico
            sb.Append($"<div id=\"{ResultContainerId}\" class=\"mt-3\"></div>");

            // ✅ Script dinamico
            sb.Append("<script>");
            if (ResetButton)
            {
                sb.Append("$(\"#accordionPanelsRicercae\").find(\"button[type='reset']\").on(\"click\", function () {");
                sb.Append("$(\"#accordionPanelsRicercae\").find(\"input[type='hidden']\").val('');");
                sb.Append("$(\"#accordionPanelsRicercae\").find(\".field-validation-error\").html('');");
                sb.Append("});");
            }

            sb.Append(UpdateListRicercaScript(RicercaAction));

            if (ExecuteRicercaOnReady)
            {
                sb.Append("$(document).ready(function() {");
                sb.Append("alertWaid();");
                sb.Append("updateListRicercaFromModuloRicerca(true);");
                sb.Append("panelRicercaCollapse(false);");
                sb.Append("});");
            }

            if (hasPartial)
            {
                sb.Append(@"
                    var panel = $('#panelRicerca-collapseOne');

                    function panelRicercaCollapse(collapse) {
                        if (!collapse) {
                            panel.collapse('hide');
                        } else {
                            panel.collapse('show');
                        }
                    }
                ");
            }

            sb.Append("</script>");

            output.TagName = "div";
            output.Content.SetHtmlContent(sb.ToString());
        }

        private string UpdateListRicercaScript(string actionUrl)
        {
            return $@"
                function updateListRicercaFromModuloRicerca(ajaxCall) {{
                    $.ajax({{
                        url: '{actionUrl}',
                        type: 'POST',
                        success: function (result) {{
                            $('#{ResultContainerId}').html(result);
                            alertClose();
                        }}
                    }});
                }}";
        }
    }
}
