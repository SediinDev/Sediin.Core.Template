using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Sediin.Core.Mvc.Helpers.Security;
using System.Text.Encodings.Web;
using System.Web;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    public class WizardContext
    {
        public List<WizardStepItem> Steps { get; } = new();
    }

    public class WizardStepItem
    {
        public string? Title { get; set; }
        public string? Action { get; set; }
        public Dictionary<string, string?> RouteValues { get; set; } = new();
        public bool Active { get; set; }
        public string? CssClass { get; set; }
        public string? DataId { get; set; }
        public IDictionary<string, string?> DataAttributes { get; } = new Dictionary<string, string?>();
    }

    [HtmlTargetElement("wizard-nav")]
    public class WizardNavTagHelper : TagHelper
    {
        private const string DefaultBaseClass = "nav nav-wizard";

        [HtmlAttributeName("class")] public string? Class { get; set; }
        [HtmlAttributeName("id")] public string? Id { get; set; }
        [HtmlAttributeName("target-id")] public string TargetId { get; set; } = "wizardContent";

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var wizardContext = new WizardContext();
            context.Items[typeof(WizardContext)] = wizardContext;

            await output.GetChildContentAsync();

            output.TagName = "ul";
            var classes = string.IsNullOrWhiteSpace(Class) ? DefaultBaseClass : $"{DefaultBaseClass} {Class}";
            output.Attributes.SetAttribute("class", classes);
            if (!string.IsNullOrWhiteSpace(Id)) output.Attributes.SetAttribute("id", Id);

            var html = new System.Text.StringBuilder();
            foreach (var step in wizardContext.Steps)
            {
                var li = new TagBuilder("li");
                var liClasses = new List<string> { "btnRichiestaNavWizard" };
                if (!string.IsNullOrWhiteSpace(step.CssClass)) liClasses.Add(step.CssClass);
                if (step.Active) liClasses.Add("active");
                li.AddCssClass(string.Join(" ", liClasses));

                if (!string.IsNullOrWhiteSpace(step.Action))
                    li.Attributes["data-action"] = step.Action!;

                if (step.RouteValues.Any())
                {
                    var queryString = string.Join("&", step.RouteValues.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));
                    var encrypted = CryptoHelper.Encrypt(queryString);
                    var encryptedParam = HttpUtility.UrlEncode(encrypted);
                    li.Attributes["data-query"] = encryptedParam;
                }

                if (!string.IsNullOrWhiteSpace(step.DataId))
                    li.Attributes["data-id"] = step.DataId!;

                foreach (var kv in step.DataAttributes)
                    if (!string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrWhiteSpace(kv.Value))
                        li.Attributes[$"data-{kv.Key}"] = kv.Value;

                li.InnerHtml.Append(step.Title ?? string.Empty);

                using var writer = new System.IO.StringWriter();
                li.WriteTo(writer, HtmlEncoder.Default);
                html.AppendLine(writer.ToString());
            }

            output.Content.SetHtmlContent(html.ToString());

            // JS integrato
            var script = $@"
<script>
document.addEventListener('DOMContentLoaded', function() {{
    document.querySelectorAll('.btnRichiestaNavWizard').forEach(function(el) {{
        el.addEventListener('click', function() {{
            var action = el.getAttribute('data-action');
            var query = el.getAttribute('data-query');
            var target = document.getElementById('{TargetId}');
            if(!action || !target) return;

            // Rimuovi active precedente
            document.querySelectorAll('.btnRichiestaNavWizard').forEach(x => x.classList.remove('active'));
            el.classList.add('active');

            fetch(action + '?p=' + encodeURIComponent(query))
                .then(r => r.text())
                .then(html => {{
                    target.innerHTML = html;
                }})
                .catch(err => console.error('Errore caricamento wizard step', err));
        }});
    }});
}});
</script>";
            output.PostContent.AppendHtml(script);
        }
    }

    [HtmlTargetElement("wizard-step", ParentTag = "wizard-nav")]
    public class WizardStepTagHelper : TagHelper
    {
        [HtmlAttributeName("title")] public string? Title { get; set; }
        [HtmlAttributeName("action")] public string? Action { get; set; }
        [HtmlAttributeName("route-values")] public Dictionary<string, string?> RouteValues { get; set; } = new();
        [HtmlAttributeName("active")] public bool Active { get; set; }
        [HtmlAttributeName("class")] public string? Class { get; set; }
        [HtmlAttributeName("data-id")] public string? DataId { get; set; }
        [HtmlAttributeName("data-additional")] public string? DataAdditional { get; set; }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!context.Items.TryGetValue(typeof(WizardContext), out var ctxObj) || ctxObj is not WizardContext wizardContext)
                return Task.CompletedTask;

            var item = new WizardStepItem
            {
                Title = Title,
                Action = Action,
                RouteValues = RouteValues ?? new(),
                Active = Active,
                CssClass = Class,
                DataId = DataId
            };

            if (!string.IsNullOrWhiteSpace(DataAdditional))
            {
                var parts = DataAdditional.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var p in parts)
                {
                    var kv = p.Split(':', 2, StringSplitOptions.TrimEntries);
                    if (kv.Length == 2)
                    {
                        var key = kv[0].Replace(" ", "-").ToLowerInvariant();
                        item.DataAttributes[key] = kv[1];
                    }
                }
            }

            wizardContext.Steps.Add(item);
            output.SuppressOutput();
            return Task.CompletedTask;
        }
    }

}
