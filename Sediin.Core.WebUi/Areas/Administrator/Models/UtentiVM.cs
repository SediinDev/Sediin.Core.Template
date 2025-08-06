using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;

namespace Sediin.Core.WebUi.Areas.Administrator.Models
{
    public class UtentiVM
    {
        public IEnumerable<SediinIdentityUser> Result { get; set; } = new List<SediinIdentityUser>();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public UtentiRicercaVM? Filtri { get; set; }
    }
}
