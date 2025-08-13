using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.RepositoryPattern;

namespace Sediin.Core.DataAccess.Repository
{
    public class UnitOfWorkDataAccess : IUnitOfWorkDataAccess
    {
        private readonly SediinCoreDataAccessDbContext _db;

        public IMenuRepository Menu { get; private set; }
        
        public IRepository<SediinCoreDataAccessDbContext> Repository { get; private set; }

        public UnitOfWorkDataAccess(SediinCoreDataAccessDbContext db)
        {
            _db = db;
            Repository = new Repository<SediinCoreDataAccessDbContext>(_db);
            Menu = new MenuRepository(Repository);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
