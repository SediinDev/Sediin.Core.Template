using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Repository;
using Sediin.Core.RepositoryPattern;

namespace Sediin.Core.DataAccess
{
    public interface IUnitOfWorkDataAccess : IUnitOfWork<SediinCoreDataAccessDbContext>
    {
        IMenuRepository Menu { get; }
    }

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
