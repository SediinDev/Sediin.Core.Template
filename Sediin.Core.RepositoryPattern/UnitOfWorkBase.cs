using Microsoft.EntityFrameworkCore;
using Sediin.Core.RepositoryPattern;

namespace Sediin.Core.DataAccess.Repository
{
    public abstract class UnitOfWorkBase<TContext> : IUnitOfWork<TContext>, IDisposable
        where TContext : DbContext
    {
        protected readonly TContext _db;
        public IRepository<TContext> Repository { get; }

        protected UnitOfWorkBase(TContext db)
        {
            _db = db;
            Repository = new Repository<TContext>(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
