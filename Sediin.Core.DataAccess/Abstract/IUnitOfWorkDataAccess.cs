using Sediin.Core.DataAccess.Data;
using Sediin.Core.RepositoryPattern;

namespace Sediin.Core.DataAccess.Abstract
{
    public interface IUnitOfWorkDataAccess
    {
        IMenuRepository Menu { get; }
        IRepository<SediinCoreDataAccessDbContext> Repository { get; }
        void Save();
    }
}
