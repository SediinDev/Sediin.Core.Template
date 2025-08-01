using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Sediin.Core.Identity.Data
{
    public class SediinCoreIdentityDbContext : IdentityDbContext
    {
        public SediinCoreIdentityDbContext(DbContextOptions<SediinCoreIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
