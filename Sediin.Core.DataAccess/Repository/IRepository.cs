using System.Linq.Expressions;

namespace Sediin.Core.DataAccess.Repository
{
    /// <summary>
    /// Repo Pattern Interface per le operazioni CRUD
    /// non neccessario implementare, Create, Read, Update, Delete
    /// esempio implementazione su repo:
    /// public class MenuRepository : Repository<Menu>, IMenuRepository
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
