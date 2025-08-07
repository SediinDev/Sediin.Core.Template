using Sediin.Core.Identity.Entities;
using Sediin.Core.Identity.Models;
using Sediin.Core.Mvc.Helpers.PagingHelpers;

namespace Sediin.Core.WebUi.Areas.Administrator.Models
{
    public class UtentiVM : PagedResultViewModel<SediinIdentityUser, UtentiRicercaVM>
    {
        public IEnumerable<SediinIdentityUser> Result { get; set; } = new List<SediinIdentityUser>();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public UtentiRicercaVM? Filtri { get; set; }
    }
}
