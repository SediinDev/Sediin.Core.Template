using Sediin.Core.DataAccess.Abstract;
using Sediin.Core.DataAccess.Data;
using Sediin.Core.DataAccess.Entities;
using Sediin.Core.RepositoryPattern;
using System.Linq.Expressions;

namespace Sediin.Core.DataAccess.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly IRepository<SediinCoreDataAccessDbContext> _repo;

        public MenuRepository(IRepository<SediinCoreDataAccessDbContext> repo)
        {
            _repo = repo;
        }

        // Metodo generico con filtro opzionale
        public async Task<IEnumerable<Menu>> GetAll(Expression<Func<Menu, bool>>? filter = null)
        {
            return await _repo.GetAllAsync<Menu>(filter, "Ruoli");
        }
    }
}
