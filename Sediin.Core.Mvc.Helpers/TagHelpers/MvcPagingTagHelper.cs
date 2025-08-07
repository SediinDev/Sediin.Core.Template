using Microsoft.AspNetCore.Razor.TagHelpers;
using Sediin.Core.Mvc.Helpers.Security;
using System.Text;
using System.Web;

namespace Sediin.Core.Mvc.Helpers.TagHelpers
{
    [HtmlTargetElement("paging-ajax")]
    public class PagingAjaxTagHelper : TagHelper
    {
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 1;
        public int TotalRows { get; set; } = 10;

        public string Action { get; set; }
        public string ActionExport { get; set; }

        public string UpdateTargetId { get; set; }

        public string HttpMethod { get; set; } = "GET";

        public string OnBegin { get; set; }
        public string OnSuccess { get; set; }
        public string OnFailure { get; set; }

        [HtmlAttributeName("query-model")]
        public object? QueryModel { get; set; }

        string uniqueId = Guid.NewGuid().ToString("N");

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            int pageSize = PageSize <= 0 ? 10 : PageSize;
            int pageIndex = PageIndex <= 0 ? 1 : PageIndex;
            int totalRows = TotalRows < 0 ? 0 : TotalRows;

            int totalPages = (int)Math.Ceiling((double)totalRows / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (pageIndex > totalPages) pageIndex = totalPages;

            string BuildUrl(string baseUrl, int? page = null)
            {
                var parameters = new List<string>();

                if (QueryModel != null)
                {
                    var props = QueryModel.GetType().GetProperties();
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(QueryModel);
                        if (value != null)
                        {
                            string val = HttpUtility.UrlEncode(value.ToString());
                            parameters.Add($"{prop.Name}={val}");
                        }
                    }
                }

                if (page.HasValue)
                {
                    parameters.Add($"page={page.Value}");
                }

                var queryString = string.Join("&", parameters);
                var encrypted = HttpUtility.UrlEncode(CryptoHelper.Encrypt(queryString));

                return $"{baseUrl}?q={encrypted}";
            }

            int pageGroupSize = 10;
            int currentGroup = (int)Math.Ceiling((double)pageIndex / pageGroupSize);
            int startPage = (currentGroup - 1) * pageGroupSize + 1;
            int endPage = Math.Min(startPage + pageGroupSize - 1, totalPages);

            var sb = new StringBuilder();

            sb.AppendLine("<div class=\"card mt-2 mb-4\">");
            sb.AppendLine("  <div class=\"card-block ml-2 mr-2 mt-3\">");
            sb.AppendLine("    <div class=\"row\">");

            sb.AppendLine("      <div class=\"col-md-6 table-responsive\" style=\"margin-top:-10px\">");
            sb.AppendLine("        <nav aria-label=\"Page navigation\">");
            sb.AppendLine("          <ul class=\"pagination\">");

            if (startPage > 1)
            {
                string prevUrl = BuildUrl(Action, startPage - 1);
                sb.AppendLine($"<li class=\"page-item\"><a data-ajax=\"true\" {GetAjaxAttributes()} class=\"page-link\" href=\"{prevUrl}\" aria-label=\"Precedente\"><span aria-hidden=\"true\">&laquo;</span><span class=\"sr-only\">Precedente</span></a></li>");
            }
            else
            {
                sb.AppendLine("<li class=\"page-item disabled\"><a class=\"page-link disabled\" href=\"#\" aria-label=\"Precedente\" onclick=\"return false;\"><span aria-hidden=\"true\">&laquo;</span><span class=\"sr-only\">Precedente</span></a></li>");
            }

            for (int i = startPage; i <= endPage; i++)
            {
                if (i == pageIndex)
                {
                    sb.AppendLine($"<li class=\"page-item active\"><a class=\"page-link\" href=\"#\" onclick=\"return false;\" style=\"cursor:default\">{i}</a></li>");
                }
                else
                {
                    string pageUrl = BuildUrl(Action, i);
                    sb.AppendLine($"<li class=\"page-item\"><a data-ajax=\"true\" {GetAjaxAttributes()} class=\"page-link\" href=\"{pageUrl}\">{i}</a></li>");
                }
            }

            if (endPage < totalPages)
            {
                string nextUrl = BuildUrl(Action, endPage + 1);
                sb.AppendLine($"<li class=\"page-item\"><a data-ajax=\"true\" {GetAjaxAttributes()} class=\"page-link\" href=\"{nextUrl}\" aria-label=\"Prossima\"><span aria-hidden=\"true\">&raquo;</span><span class=\"sr-only\">Prossima</span></a></li>");
            }
            else
            {
                sb.AppendLine("<li class=\"page-item disabled\"><a class=\"page-link disabled\" href=\"#\" aria-label=\"Prossima\" onclick=\"return false;\"><span aria-hidden=\"true\">&raquo;</span><span class=\"sr-only\">Prossima</span></a></li>");
            }

            sb.AppendLine("          </ul>");
            sb.AppendLine("        </nav>");
            sb.AppendLine("      </div>");

            if (!string.IsNullOrWhiteSpace(ActionExport))
            {
                string exportUrl = BuildUrl(ActionExport);
                sb.AppendLine("      <div class=\"col-md-2\">");
                sb.AppendLine($"        <a href=\"{exportUrl}\" target=\"_blank\" class=\"btn btn-primary btn-sm\" style=\"margin-top:-10px\"><i class=\"fas fa-file-excel mr-2\"></i>Esporta Excel</a>");
                sb.AppendLine("      </div>");
            }
            else
            {
                sb.AppendLine("      <div class=\"col-md-2\"></div>");
            }

            sb.AppendLine("      <div class=\"col-md-1 form-inline text-right pull-right\" style=\"margin-top:-10px\">");
            sb.AppendLine($"        <input onfocus=\"this.select()\" type=\"number\" id=\"pagingInput_{uniqueId}\" name=\"pagingInput_{uniqueId}\" class=\"form-control form-control-sm col-md-6\" placeholder=\"Pagina\" min=\"1\" max=\"{totalPages}\" value=\"{pageIndex}\">");
            sb.AppendLine("      </div>");

            sb.AppendLine("      <div class=\"col-md-1 form-inline text-right pull-right\" style=\"margin-top:-10px\">");
            sb.AppendLine($"        <button type=\"button\" class=\"btn btn-sm btn-primary ml-1\" onclick=\"gotoPage_{uniqueId}()\">Vai</button>");
            sb.AppendLine("      </div>");

            int recordStart = ((pageIndex - 1) * pageSize) + 1;
            int recordEnd = pageIndex * pageSize > totalRows ? totalRows : pageIndex * pageSize;

            sb.AppendLine("      <div class=\"col-md-2 text-right pull-right mb-2\" style=\"margin-top:-20px\">");
            sb.AppendLine("        <div><small>");
            sb.AppendLine($"          Record <strong>{recordStart}</strong> / <strong>{recordEnd}</strong>");
            sb.AppendLine("        </small></div>");
            sb.AppendLine("        <div><small>");
            sb.AppendLine($"          Totale record: <strong>{totalRows}</strong>");
            sb.AppendLine("        </small></div>");
            sb.AppendLine("        <div><small>");
            sb.AppendLine($"          Totale pagine: <strong>{totalPages}</strong>");
            sb.AppendLine("        </small></div>");
            sb.AppendLine("      </div>");

            sb.AppendLine("    </div>");
            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            // Javascript per "Vai alla pagina"
            sb.AppendLine("<script>");
            sb.AppendLine("  function gotoPagePaging_" + uniqueId + "() { }");

            sb.AppendLine("  function gotoPage_" + uniqueId + "() {");
            sb.AppendLine($"    var val = parseInt(document.getElementById('pagingInput_{uniqueId}').value);");
            sb.AppendLine($"    if (isNaN(val) || val < 1 || val > {totalPages}) {{ toastInfo('Inserire un numero pagina valido da 1 a {totalPages}.'); return; }}");

            var jqMethod = (HttpMethod ?? "GET").ToUpperInvariant() == "POST" ? "post" : "get";
            var baseUrl = BuildUrl(Action);

            sb.AppendLine($"    $.{jqMethod}('{baseUrl}', {{ page: val }}, function(data) {{");
            if (!string.IsNullOrWhiteSpace(UpdateTargetId))
                sb.AppendLine($"      $('#{UpdateTargetId}').html(data);");
            sb.AppendLine("    });");

            sb.AppendLine("  }");
            sb.AppendLine("</script>");

            sb.AppendLine("<script>");
            sb.AppendLine("  setTimeout(function() {");
            sb.AppendLine("    var hiddenInput = document.querySelector('input[data-ricercamodulo-page-number]');");
            sb.AppendLine($"    if(hiddenInput) hiddenInput.value = {pageIndex};");
            sb.AppendLine("  }, 100);");
            sb.AppendLine("</script>");

            output.TagName = null;
            output.Content.SetHtmlContent(sb.ToString());
        }

        private string GetAjaxAttributes()
        {
            var sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(HttpMethod))
                sb.Append($" data-ajax-method=\"{HttpMethod}\" ");

            if (!string.IsNullOrWhiteSpace(OnBegin))
                sb.Append($" data-ajax-begin=\"{OnBegin}\" ");
            else
                sb.Append($" data-ajax-begin=\"gotoPagePaging_{uniqueId}()\" ");

            if (!string.IsNullOrWhiteSpace(OnSuccess))
                sb.Append($" data-ajax-success=\"{OnSuccess}\" ");

            if (!string.IsNullOrWhiteSpace(OnFailure))
                sb.Append($" data-ajax-failure=\"{OnFailure}\" ");

            if (!string.IsNullOrWhiteSpace(UpdateTargetId))
                sb.Append($" data-ajax-mode=\"replace\" data-ajax-update=\"#{UpdateTargetId}\" ");

            return sb.ToString();
        }
    }
}
