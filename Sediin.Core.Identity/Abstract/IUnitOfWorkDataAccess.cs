namespace Sediin.Core.Identity.Abstract
{
    public interface IUnitOfWorkIdentity
    {
        IAuthService AuthService{ get; }
    }
}
