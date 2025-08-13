using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;

namespace Sediin.Core.DataAccess.Repository
{
    public class UnitOfWorkDataAccess : UnitOfWorkBase<SediinCoreDataAccessDbContext>, IUnitOfWorkDataAccess
    {
        public IMenuRepository Menu { get; }

        public UnitOfWorkDataAccess(SediinCoreDataAccessDbContext db)
            : base(db)
        {
            Menu = new MenuRepository(Repository);
        }
    }
}
