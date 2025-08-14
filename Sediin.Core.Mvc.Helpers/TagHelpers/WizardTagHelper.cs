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

        // true => su errore torna al tab precedente; false => resta sul tab corrente
        [HtmlAttributeName("asp-error-select-last-tab")]
        public bool ErrorSelectLastTab { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            if (!output.Attributes.TryGetAttribute("class", out var _))
                output.Attributes.SetAttribute("class", "nav nav-wizard");

            // usa l'id presente o generane uno
            var existingId = output.Attributes["id"]?.Value?.ToString();
            var id = string.IsNullOrWhiteSpace(existingId) ? $"wizard_{Guid.NewGuid():N}" : existingId!;
            output.Attributes.SetAttribute("id", id);

            var httpMethod = (Method ?? "GET").ToUpperInvariant() == "POST" ? "POST" : "GET";

            var script = $@"
<script>
document.addEventListener('DOMContentLoaded', function() {{
  var wizard = document.getElementById('{id}');
  if (!wizard) return;
  var targetDiv = document.getElementById('{TargetDivId}');
  if (!targetDiv) return;

  var lastActiveIndex = null;

  function handleClick(li) {{
    var lis = wizard.querySelectorAll('li');
    lastActiveIndex = Array.prototype.findIndex.call(lis, function(x) {{
      return x.classList.contains('active');
    }});

    // attiva il tab cliccato
    lis.forEach(function(x) {{ x.classList.remove('active'); }});
    li.classList.add('active');

    var url = li.getAttribute('data-url');
    if (!url) return;

    // loading dinamico
    targetDiv.innerHTML =
      '<div class=""loading_outer text-center p-5"">' +
        '<div class=""spinner-border mt-3 {LoadingClass}""></div>' +
        '<div class=""{LoadingClass} mt-3""><strong>{LoadingText}</strong></div>' +
      '</div>';

    {(string.IsNullOrWhiteSpace(OnBegin) ? "" : $"{OnBegin}(li);")}

    fetch(url, {{
      method: '{httpMethod}',
      headers: {{ 'X-Requested-With': 'XMLHttpRequest' }}
    }})
    .then(function(response) {{
      if (!response.ok) throw response;
      return response.text();
    }})
    .then(function(html) {{
      targetDiv.innerHTML = html;
      {(string.IsNullOrWhiteSpace(OnSuccess) ? "" : $"{OnSuccess}(html, li);")}
    }})
    .catch(function(err) {{
      function showErrorMessage(msg) {{
        targetDiv.innerHTML = '<div class=""alert alert-danger mt-3 p-3"">' + msg + '</div>';
      }}

      var fallback = 'Si è verificato un errore imprevisto.';
      if (err instanceof Response) {{
        err.text().then(function(text) {{
          var msg = fallback;
          try {{
            var json = text ? JSON.parse(text) : null;
            if (json && typeof json.message === 'string' && json.message.trim() !== '') {{
              msg = json.message;
            }} else {{
              msg = text || fallback;
            }}
          }} catch (_e) {{
            msg = text || fallback;
          }}
          showErrorMessage(msg);
          {(string.IsNullOrWhiteSpace(OnFailure) ? "" : $"{OnFailure}({{ message: msg, status: err.status }}, li);")}
        }});
      }} else {{
        var msg = (err && err.message) ? err.message : fallback;
        showErrorMessage(msg);
        {(string.IsNullOrWhiteSpace(OnFailure) ? "" : $"{OnFailure}({{ message: msg }}, li);")}
      }}

      // gestione tab in caso di errore
      {(ErrorSelectLastTab
        ? @"var liList = wizard.querySelectorAll('li');
            liList.forEach(function(x) {{ x.classList.remove('active'); }});
            if (lastActiveIndex !== null && lastActiveIndex >= 0 && lastActiveIndex < liList.length) {{
              liList[lastActiveIndex].classList.add('active');
            }}"
        : @"/* resta sul tab corrente (cliccato) in caso di errore */")}
    }})
    .finally(function() {{
      {(string.IsNullOrWhiteSpace(OnComplete) ? "" : $"{OnComplete}(li);")}
    }});
  }}

  // listener sui <li>
  wizard.querySelectorAll('li').forEach(function(li) {{
    li.addEventListener('click', function() {{ handleClick(li); }});
  }});

  // funzione globale per cambiare tab da JS
  window.setWizardTab = function(wizardId, tabIndex) {{
    var w = document.getElementById(wizardId);
    if (!w) return;
    var lis = w.querySelectorAll('li');
    if (tabIndex < 0 || tabIndex >= lis.length) {{
      console.error('Indice tab non valido:', tabIndex);
      return;
    }}
    lis[tabIndex].click();
  }};

  // auto-run: tab con .active, altrimenti il primo
  var initial = wizard.querySelector('li.active') || wizard.querySelector('li');
  if (initial) initial.click();
}});
</script>";

            output.PostElement.AppendHtml(script);
        }
    }
}
