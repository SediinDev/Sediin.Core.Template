using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.DataAccess.Abstract
{
    public interface IUnitOfWorkDataAccess
    {
        IAziendaRepository Aziende { get; }
        IMenuRepository Menu{ get; }
        void Save();

    }
}
