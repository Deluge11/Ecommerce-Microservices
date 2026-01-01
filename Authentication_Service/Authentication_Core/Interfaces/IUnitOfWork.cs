using Authentication_Core.Entities;


namespace Authentication_Core.Interfaces
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IRepositoryBase<Account> Accounts { get; }
        IRepositoryBase<Role> Roles { get; }
        IRepositoryPermissions Permissions { get; }
        Task<int> Complete();
    }
}
