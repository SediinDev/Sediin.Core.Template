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

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await _db.Set<TEntity>().AddAsync(entity);
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Update(entity);
            await Task.CompletedTask; // EF Update è sincrono, lasciamo compatibilità async
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _db.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAllAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _db.Set<TEntity>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        public async Task<TEntity?> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class
        {
            var query = await GetAllAsync(predicate, includeProperties, tracked, null, null);
            return query.Items.FirstOrDefault();
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllAsync<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false,
            int? pageNumber = null,
            int? pageSize = null
        ) where TEntity : class
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

            int totalCount = await query.CountAsync();

            if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
            {
                int skip = (pageNumber.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            var items = await query.ToListAsync();

            return (items, totalCount);
        }

    //    public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
    //        Expression<Func<TEntity, bool>>? predicate = null,
    //        string? includeProperties = null,
    //        bool tracked = false,
    //        int? pageNumber = null,
    //        int? pageSize = null) where TEntity : class
    //    {
    //        IQueryable<TEntity> query = tracked
    //            ? _db.Set<TEntity>()
    //            : _db.Set<TEntity>().AsNoTracking();

    //        if (predicate != null)
    //            query = query.Where(predicate);

    //        if (!string.IsNullOrWhiteSpace(includeProperties))
    //        {
    //            foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
    //            {
    //                query = query.Include(includeProperty);
    //            }
    //        }

    //        if (pageNumber.HasValue && pageSize.HasValue && pageNumber > 0 && pageSize > 0)
    //        {
    //            int skip = (pageNumber.Value - 1) * pageSize.Value;
    //            query = query.Skip(skip).Take(pageSize.Value);
    //        }

    //        return await query.ToListAsync();
    //    }
    }
}
