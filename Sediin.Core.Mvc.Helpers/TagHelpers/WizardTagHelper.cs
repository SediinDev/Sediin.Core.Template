using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("wizard", Attributes = "asp-target")]
    public class WizardTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-target")]
        public string TargetDivId { get; set; }

        [HtmlAttributeName("asp-method")]
        public string Method { get; set; } = "GET";

        [HtmlAttributeName("on-begin")]
        public string OnBegin { get; set; }

        [HtmlAttributeName("on-success")]
        public string OnSuccess { get; set; }

        [HtmlAttributeName("on-failure")]
        public string OnFailure { get; set; }

        [HtmlAttributeName("on-complete")]
        public string OnComplete { get; set; }

        [HtmlAttributeName("asp-loading-text")]
        public string LoadingText { get; set; } = "Caricamento in corso...";

        [HtmlAttributeName("asp-loading-class")]
        public string LoadingClass { get; set; } = "text-primary";

        [HtmlAttributeName("asp-error-select-last-tab")]
        public bool ErrorSelectLastTab { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            if (!output.Attributes.TryGetAttribute("class", out var _))
                output.Attributes.SetAttribute("class", "nav nav-wizard");

            var existingId = output.Attributes["id"]?.Value?.ToString();
            var id = string.IsNullOrWhiteSpace(existingId) ? $"wizard_{Guid.NewGuid():N}" : existingId!;
            output.Attributes.SetAttribute("id", id);

            var httpMethod = (Method ?? "GET").ToUpperInvariant() == "POST" ? "POST" : "GET";

            var script = $@"
<script>
$(function() {{
    var wizard = $('#{id}');
    var targetDiv = $('#{TargetDivId}');
    var lastActiveIndex = null;

    function handleClick(li) {{
        if ($(li).data('disabled') === true) return;

        // VALIDAZIONE SOLO SE IL TAB CLICCATO HA data-require-validation=""true""
        if ($(li).data('require-validation') === true) {{
            var form = targetDiv.find('form').first();
            if (form.length > 0 && $.validator && !form.valid()) {{
                console.warn('Form non valido. Impossibile cambiare tab.');
                return;
            }}
        }}

        var lis = wizard.find('li');
        lastActiveIndex = lis.index(lis.filter('.active'));
        lis.removeClass('active');
        $(li).addClass('active');

        var url = $(li).data('url');
        if (!url) return;

        targetDiv.html(
            '<div class=""loading_outer text-center p-5"">' +
            '<div class=""spinner-border mt-3 {LoadingClass}""></div>' +
            '<div class=""{LoadingClass} mt-3""><strong>{LoadingText}</strong></div>' +
            '</div>'
        );

        {(string.IsNullOrWhiteSpace(OnBegin) ? "" : $"{OnBegin}(li);")}

        $.ajax({{
            url: url,
            type: '{httpMethod}',
            headers: {{ 'X-Requested-With': 'XMLHttpRequest' }},
            success: function(html) {{
                targetDiv.html(html);

                if ($.validator) {{
                    $.validator.unobtrusive.parse(targetDiv);
                }}

                {(string.IsNullOrWhiteSpace(OnSuccess) ? "" : $"{OnSuccess}(html, li);")}
            }},
            error: function(xhr) {{
                var msg = 'Si è verificato un errore imprevisto.';
                try {{
                    var res = xhr.responseText ? JSON.parse(xhr.responseText) : null;
                    if (res && res.message) msg = res.message;
                    else if (xhr.responseText && xhr.responseText.trim() !== '') msg = xhr.responseText;
                }} catch (_e) {{
                    if (xhr.responseText && xhr.responseText.trim() !== '') msg = xhr.responseText;
                }}

                targetDiv.html('<div class=""alert alert-danger mt-3 p-3"">' + msg + '</div>');

                // chiama asp-failure passando xhr originale
                if (window['{OnFailure}']) {{
                    window['{OnFailure}'](xhr, li);
                }}

                if ({ErrorSelectLastTab.ToString().ToLower()}) {{
                    lis.removeClass('active');
                    if (lastActiveIndex !== null && lastActiveIndex >= 0 && lastActiveIndex < lis.length)
                        lis.eq(lastActiveIndex).addClass('active');
                }}
            }},
            complete: function() {{
                {(string.IsNullOrWhiteSpace(OnComplete) ? "" : $"{OnComplete}(li);")}
            }}
        }});
    }}

    wizard.find('li').on('click', function() {{
        handleClick(this);
    }});

    window.setWizardTab = function(wizardId, tabIndex) {{
        var w = $('#' + wizardId);
        if (!w.length) return;
        var lis = w.find('li');
        if (tabIndex < 0 || tabIndex >= lis.length) return;
        lis.eq(tabIndex).click();
    }};

    var initial = wizard.find('li.active').first();
    if (!initial.length) initial = wizard.find('li').first();
    if (initial.length) initial.click();
}});
</script>";

            output.PostElement.AppendHtml(script);
        }
    }
}
