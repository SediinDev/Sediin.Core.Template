using Microsoft.AspNetCore.Builder;

namespace Sediin.Core.Mvc.Helpers.Middleware
{
    public static class QueryDecryptMiddlewareExtensions
    {
        public static IApplicationBuilder UseQueryDecrypt(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<QueryDecryptMiddleware>();
        }
    }
}
