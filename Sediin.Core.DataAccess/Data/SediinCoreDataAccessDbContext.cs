using Microsoft.EntityFrameworkCore;
using Sediin.Core.DataAccess.Entities;

namespace Sediin.Core.DataAccess.Data
{
    public class SediinCoreDataAccessDbContext : DbContext
    {
        #pragma warning disable
        public SediinCoreDataAccessDbContext(DbContextOptions<SediinCoreDataAccessDbContext> options)
            : base(options)
        {
        }

        public DbSet<Azienda> Aziende { get; set; }

    }
}
