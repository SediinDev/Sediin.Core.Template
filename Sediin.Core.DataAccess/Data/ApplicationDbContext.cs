using Microsoft.EntityFrameworkCore;

namespace Sediin.Core.DataAccess.Data
{
    public class SediinCoreDataAccessDbContext : DbContext
    {
        public SediinCoreDataAccessDbContext(DbContextOptions<SediinCoreDataAccessDbContext> options)
            : base(options)
        {
        }
    }
}
