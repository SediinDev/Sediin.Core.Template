using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Entities;

namespace Sediin.Core.DataAccess.Repository
{
    public class AziendaRepository : Repository<Azienda>, IAziendaRepository
    {
        SediinCoreDataAccessDbContext _db;

        public AziendaRepository(SediinCoreDataAccessDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
