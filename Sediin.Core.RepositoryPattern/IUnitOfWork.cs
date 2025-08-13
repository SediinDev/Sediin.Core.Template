using Microsoft.EntityFrameworkCore;

namespace Sediin.Core.RepositoryPattern
{
    public interface IUnitOfWork<TContext> where TContext : DbContext
    {
        IRepository<TContext> Repository { get; }
        void Save();
        Task SaveAsync();
    }
}
