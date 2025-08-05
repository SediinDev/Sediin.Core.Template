using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;

namespace Sediin.Core.DataAccess.Repository
{
    public class UnitOfWorkDataAccess : IUnitOfWorkDataAccess
    {
        SediinCoreDataAccessDbContext _db;

        public IAziendaRepository Aziende { get; private set; }

        public IMenuRepository Menu { get; private set; }

        public UnitOfWorkDataAccess(SediinCoreDataAccessDbContext db)
        {
            _db = db;
            Aziende = new AziendaRepository(_db);
            Menu = new MenuRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
