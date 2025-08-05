using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sediin.Core.Identity.Entities;

namespace Sediin.Core.Identity.Data
{
    public class SediinCoreIdentityDbContext : IdentityDbContext<SediinIdentityUser>
    {
        #pragma warning disable
        public SediinCoreIdentityDbContext(DbContextOptions<SediinCoreIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
