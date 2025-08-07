using Microsoft.Extensions.DependencyInjection;

namespace Sediin.Core.Identity.Mapping
{
    public static class SediinIdentityAutoMapperRegistration
    {
        public static void SediinIdentityAutoMapper(this IServiceCollection services)
        {
            // Usa l'assembly della libreria corrente
            services.AddAutoMapper(typeof(SediinIdentityAutoMapperRegistration).Assembly);
        }
    }
}
