using Sediin.Core.Identity.Entities;

namespace Sediin.Core.Identity.Models
{
    public class SediinIdentityUserWithRoles
    {
        public SediinIdentityUser User { get; set; }
        public IList<string> Roles { get; set; }
    }

}
