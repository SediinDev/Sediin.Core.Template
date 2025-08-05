using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Abstract;
using Sediin.Core.Identity.Entities;

namespace Sediin.Core.Identity.Repository
{
    public class UnitOfWorkIdentity : IUnitOfWorkIdentity
    {
        public IAuthService AuthService { get; }

        public UnitOfWorkIdentity(SignInManager<SediinIdentityUser> signInManager, UserManager<SediinIdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            AuthService = new AuthService(signInManager, userManager, roleManager);
        }
    }
}
