using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Sediin.Core.RepositoryPattern
{
    public interface IRepository<TContext> where TContext : DbContext
    {
        IEnumerable<TEntity> GetAll<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class;

        TEntity Get<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class;

        void Add<TEntity>(TEntity entity) where TEntity : class;

        void Update<TEntity>(TEntity entity) where TEntity : class;

        void Delete<TEntity>(TEntity entity) where TEntity : class;

        void DeleteAll<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
    }
}
