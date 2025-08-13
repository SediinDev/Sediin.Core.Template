using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Sediin.Core.RepositoryPattern
{
    public interface IRepository<TContext> where TContext : DbContext
    {
        Task AddAsync<TEntity>(TEntity entity) where TEntity : class;

        Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;

        Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;

        Task DeleteAllAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        Task<TEntity?> GetAsync<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false) where TEntity : class;

        Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(
            Expression<Func<TEntity, bool>>? predicate = null,
            string? includeProperties = null,
            bool tracked = false,
            int? pageNumber = null,
            int? pageSize = null) where TEntity : class;
    }
}
