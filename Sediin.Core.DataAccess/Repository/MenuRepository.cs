using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Entities;

namespace Sediin.Core.DataAccess.Repository
{
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        SediinCoreDataAccessDbContext _db;

        public MenuRepository(SediinCoreDataAccessDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
