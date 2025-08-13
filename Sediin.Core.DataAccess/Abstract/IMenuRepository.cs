using Sediin.Core.DataAccess.Entities;
using System.Linq.Expressions;

namespace Sediin.Core.DataAccess.Abstract
{
    public interface IMenuRepository
    {
        IEnumerable<Menu> GetAll(Expression<Func<Menu, bool>>? filter = null);
    }
}
