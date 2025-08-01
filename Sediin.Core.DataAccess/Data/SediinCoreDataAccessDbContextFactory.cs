using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sediin.Core.DataAccess.Data
{
    /// <summary>
    /// Viene usato per cmd Migration, altrimenti migration va in errore
    /// </summary>
    public class SediinCoreDataAccessDbContextFactory : IDesignTimeDbContextFactory<SediinCoreDataAccessDbContext>
    {
        public SediinCoreDataAccessDbContext CreateDbContext(string[] args)
        {
            string conn = "Server=.; Database=Utility;Trusted_Connection=True; TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<SediinCoreDataAccessDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new SediinCoreDataAccessDbContext(optionsBuilder.Options);
        }
    }
}
