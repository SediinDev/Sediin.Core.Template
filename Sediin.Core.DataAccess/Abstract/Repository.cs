using Microsoft.EntityFrameworkCore;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Repository;
using System.Linq.Expressions;

namespace Sediin.Core.DataAccess.Abstract
{
    public class Repository<T> : IRepository<T> where T : class
    {
        SediinCoreDataAccessDbContext _db;

        internal DbSet<T> _dbSet;

        public Repository(SediinCoreDataAccessDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeleteAll(IEnumerable<T> entity)
        {
            throw new NotImplementedException();
        }

        public T Get(Expression<Func<T, bool>>? predicate = null, string? includeProperts = null, bool tracked = false)
        {
            var _q = GetAll(predicate, includeProperts, tracked);

            return _q.FirstOrDefault();
        }

        //public IEnumerable<T> GetAll()
        //{
        //    return _dbSet;
        //}

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, string? includeProperts = null, bool tracked = false)
        {
            IQueryable<T> _query;
            if (predicate != null)
            {
                _query = tracked ? _dbSet.AsNoTracking().Where(predicate) : _dbSet.Where(predicate);
            }
            else
            {
                _query = tracked ? _dbSet.AsNoTracking() : _dbSet;
            }

            if (!string.IsNullOrWhiteSpace(includeProperts))
            {
                foreach (var item in includeProperts.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    _query = _query.Include(item);
                }
            }

            return _query;
        }
    }
}
