using Microsoft.AspNetCore.Http;
using Sediin.Core.Mvc.Helpers.Security;
using System.Threading.Tasks;
using System.Web;

namespace Sediin.Core.Mvc.Helpers.Middleware
{
    public class QueryDecryptMiddleware
    {
        private readonly RequestDelegate _next;

        public QueryDecryptMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (context.Request.Query.ContainsKey("q"))
            {
                try
                {
                    var encrypted = context.Request.Query["q"].ToString();
                    var decrypted = HttpUtility.UrlDecode(CryptoHelper.Decrypt(encrypted));

                    var queryCollection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(decrypted);

                    var newQuery = new QueryString();
                    foreach (var pair in queryCollection)
                    {
                        foreach (var val in pair.Value)
                            newQuery = newQuery.Add(pair.Key, val);
                    }

                    // Ricrea la query string senza "q"
                    context.Request.QueryString = newQuery;
                }
                catch
                {
                    // Fallimento nella decryption: ignora o gestisci come 400
                }
            }

            await _next(context);
        }
    }
}
