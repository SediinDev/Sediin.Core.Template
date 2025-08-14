using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    public class SelectOptionCustom
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    [HtmlTargetElement("select-custom", Attributes = "asp-items,asp-for")]
    public class SelectCustomTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("asp-items")]
        public IEnumerable<SelectOptionCustom> Items { get; set; }

        [HtmlAttributeName("multiple")]
        public bool Multiple { get; set; } = false;

        [HtmlAttributeName("placeholder")]
        public string Placeholder { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Trasforma il tag personalizzato in una select HTML standard
            output.TagName = "select";

            // ID e Name
            var id = For.Name.Replace(".", "_");
            output.Attributes.SetAttribute("id", id);
            output.Attributes.SetAttribute("name", For.Name + (Multiple ? "[]" : ""));
            if (Multiple) output.Attributes.SetAttribute("multiple", "multiple");

            // Validazione
            var isRequired = For.Metadata.IsRequired;
            output.Attributes.SetAttribute("data-val", isRequired ? "true" : "false");
            if (isRequired)
            {
                var labelText = For.Metadata.DisplayName ?? For.Name;
                output.Attributes.SetAttribute("data-val-required", $"Il campo {labelText} è obbligatorio.");
            }

            // Valore dal modello
            var modelValue = For.Model;

            // Placeholder (solo per select singola)
            if (!string.IsNullOrEmpty(Placeholder) && !Multiple)
            {
                var ph = new TagBuilder("option");
                ph.Attributes["value"] = "";

                if (modelValue == null || string.IsNullOrEmpty(modelValue.ToString()))
                {
                    ph.Attributes["selected"] = "selected";
                }

                ph.InnerHtml.Append(Placeholder);
                output.Content.AppendHtml(ph);
            }

            // Costruzione options
            foreach (var item in Items)
            {
                var option = new TagBuilder("option");
                option.Attributes["value"] = item.Value;

                if (Multiple && modelValue is IEnumerable<string> list && list.Contains(item.Value))
                {
                    option.Attributes["selected"] = "selected";
                }
                else if (!Multiple && modelValue != null && modelValue.ToString() == item.Value)
                {
                    option.Attributes["selected"] = "selected";
                }

                option.InnerHtml.Append(item.Text);
                output.Content.AppendHtml(option);
            }
        }
    }
}
