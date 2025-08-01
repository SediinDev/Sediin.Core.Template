using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.Core.DataAccess.Abstract
{
    public interface IUnitOfWork
    {
        IAziendaRepository Aziende { get; }
        void Save();

    }
}
