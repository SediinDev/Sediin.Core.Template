using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Sediin.Core.Identity.Mapping
{
    public static class SediinIdentityAutoMapperRegistration
    {
        public static void SediinIdentityAutoMapper(this IServiceCollection services)
        {
            // Registra tutti i profili dell'assembly della libreria
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
