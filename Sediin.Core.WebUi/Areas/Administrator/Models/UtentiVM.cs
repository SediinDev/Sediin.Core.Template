using Sediin.Core.Identity.Models;
using Sediin.Core.Mvc.Helpers.PagingHelpers;

namespace Sediin.Core.WebUi.Areas.Administrator.Models
{
    public class UtentiVM : PagedResultViewModel<SediinIdentityUserWithRoles, UtentiRicercaVM>
    {
        public IEnumerable<SediinIdentityUserWithRoles> Result { get; set; } = new List<SediinIdentityUserWithRoles>();
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public UtentiRicercaVM? Filtri { get; set; }
    }
}
