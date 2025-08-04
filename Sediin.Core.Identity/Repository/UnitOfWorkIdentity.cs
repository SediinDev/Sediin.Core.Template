using Microsoft.AspNetCore.Identity;
using Sediin.Core.Identity.Abstract;

namespace Sediin.Core.Identity.Repository
{
    public class UnitOfWorkIdentity : IUnitOfWorkIdentity
    {
        public IAuthService AuthService { get; }

        public UnitOfWorkIdentity(SignInManager<IdentityUser> signInManager)
        {
            AuthService = new AuthService(signInManager);
        }
    }
}
