using System.Linq.Expressions;

namespace Sediin.Core.DataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperts = null, bool tracked = false);

        T Get(Expression<Func<T, bool>>? predicate = null, string? includeProperts = null, bool tracked = false);

        void Update(T entity);

        void Delete(T entity);

        void Add(T entity);

        void DeleteAll(IEnumerable<T> entity);
    }
}
