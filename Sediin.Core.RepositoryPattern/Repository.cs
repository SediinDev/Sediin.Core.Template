using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Sediin.Core.RepositoryPattern
{
    public class Repository<TContext> : IRepository<TContext> where TContext : DbContext
    {
        protected readonly TContext _db;

        public Repository(TContext db)
        {
            _db = db;
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Add(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Update(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Remove(entity);
        }

        public void DeleteAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Set<TEntity>().RemoveRange(entities);
        }

        public TEntity Get<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class
        {
            return GetAll(predicate, includeProperties, tracked).FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAll<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class
        {
            IQueryable<TEntity> query = tracked
                ? _db.Set<TEntity>()
                : _db.Set<TEntity>().AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return query;
        }
    }
}
