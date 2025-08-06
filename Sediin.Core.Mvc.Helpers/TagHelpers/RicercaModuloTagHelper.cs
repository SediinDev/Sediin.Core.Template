using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
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

        // Nuova proprietà per testo bottone reset
        public string ResetButtonText { get; set; } = "Reset";

        public string SubmitText { get; set; } = "Avvia ricerca";
        public IHtmlContent? PartialHtml { get; set; }
        public string ResultContainerId { get; set; } = "resultRicerca";

        private readonly string uniqueId = Guid.NewGuid().ToString("N");

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var sb = new StringBuilder();

            string accordionId = $"accordionPanelsRicercae_{uniqueId}";
            string collapseId = $"panelRicerca-collapseOne_{uniqueId}";
            string headingId = $"panelRicerca-headingOne_{uniqueId}";
            string formId = $"form_{uniqueId}";

            sb.Append($"<div class=\"accordion shadow mb-4\" id=\"{accordionId}\">");
            sb.Append("<div class=\"accordion-item\">");

            sb.Append($"<h2 class=\"accordion-header\" id=\"{headingId}\">");
            if (PartialHtml != null)
            {
                sb.Append($@"<button class=""accordion-button text-dark"" type=""button"" data-bs-toggle=""collapse"" data-bs-target=""#{collapseId}"" aria-expanded=""true"" aria-controls=""{collapseId}"">");
                sb.Append($"<h4 class=\"m-0\">{HeaderText}</h4>");
                sb.Append("</button>");
            }
            else
            {
                sb.Append($"<h4 class=\"m-3\">{HeaderText}</h4>");
            }
            sb.Append("</h2>");

            if (PartialHtml != null)
            {
                sb.Append($@"<div id=""{collapseId}"" class=""accordion-collapse collapse show"" aria-labelledby=""{headingId}"">");
                sb.Append("<div class=\"accordion-body\">");

                sb.Append($@"<form id=""{formId}"" method=""post"" action=""{RicercaAction}""
                                data-ajax=""true""
                                data-ajax-mode=""replace""
                                data-ajax-update=""#{ResultContainerId}""
                                data-ajax-begin=""alertWaid()""
                                data-ajax-complete=""alertClose()"">");

                var writer = new System.IO.StringWriter();
                PartialHtml?.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                sb.Append(writer.ToString());

                sb.Append("   <hr />");

                sb.Append(@"<div class=""d-flex justify-content-center gap-2 mt-3"">");
                // submit button con validazione
                sb.Append($@"<button type=""submit"" onclick=""return submitIfValid_{uniqueId}(event);"" class=""btn btn-primary"" id=""submitBtn_{uniqueId}"">{SubmitText}</button>");
                if (ResetButton)
                {
                    sb.Append($@"<button type=""reset"" class=""btn btn-danger"">{ResetButtonText}</button>");
                }
                sb.Append("</div>");

                sb.Append("</form>");
                sb.Append("</div>");
                sb.Append("</div>");
            }

            sb.Append("</div></div>");
            sb.Append("<hr style=\"border: 0; height: 4px; background-image: linear-gradient(to right, #0d6efd, #6610f2); border-radius: 2px; opacity: 1; margin: 1rem 0;\" />\r\n");

            sb.Append($@"<div id=""{ResultContainerId}"" class=""mt-3""></div>");

            sb.Append("<script>");
            sb.Append($@"
function fadeCollapse_{uniqueId}() {{
    $('#{collapseId}').fadeTo(150, 0, function() {{
        $(this).collapse('hide').fadeTo(0, 1);
    }});
}}

function submitIfValid_{uniqueId}(event) {{
    var form = document.getElementById('{formId}');
    if (!form.checkValidity()) {{
        event.preventDefault();
        form.reportValidity();
        return false;
    }}
    fadeCollapse_{uniqueId}();
    return true;
}}

// aggiorna lista rimuovendo temporaneamente onbegin e rimettendolo subito dopo
window.updateListRicerca = function(showalert) {{
    var form = $('#{formId}');
    var beginHandler = form.attr('data-ajax-begin'); // salva onbegin
    form.removeAttr('data-ajax-begin');             // rimuove onbegin per evitare trigger automatico
    if (showalert) {{
        alertWaid();  // fai partire manualmente alert
    }}
    form.submit();                                    // submit senza onbegin
    setTimeout(function() {{
        form.attr('data-ajax-begin', beginHandler);  // rimetti onbegin dopo submit
    }}, 100);
}};
");

            sb.Append(@"
(function waitForJQuery() {
    if (window.jQuery) {
        (function($) {
            $(function() {");

            if (ResetButton)
            {
                sb.Append($@"
                $('#{accordionId}').find('button[type=reset]').on('click', function() {{
                    $('#{accordionId}').find('input[type=hidden]').val('');
                    $('#{accordionId}').find('.field-validation-error').html('');
                }});");
            }

            if (ExecuteRicercaOnReady)
            {
                sb.Append($@"
                setTimeout(function() {{
                    updateListRicerca(true);
                    fadeCollapse_{uniqueId}();
                }}, 300);");
            }

            sb.Append(@"
            });
        })(window.jQuery);
    } else {
        setTimeout(waitForJQuery, 50);
    }
})();
");
            sb.Append("</script>");

            output.TagName = "div";
            output.Content.SetHtmlContent(sb.ToString());
        }
    }
}
