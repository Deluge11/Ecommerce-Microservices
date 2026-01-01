using Authentication_Core.Entities;
using Authentication_Core.Interfaces;
using Authentication_Core.Repositories;
using Authentication_Infrastructure.ApplicationDbContext;
using Authentication_Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(AppDbContext context)
        {
            Accounts = new RepositoryBase<Account>(context);
            Roles = new RepositoryBase<Role>(context);
            Permissions = new RepositoryPermission(context);
            Context = context;
        }

        public IRepositoryBase<Account> Accounts { get; private set; }
        public IRepositoryBase<Role> Roles { get; private set; }
        public IRepositoryPermissions Permissions { get; private set; }
        public AppDbContext Context { get; }

        public async Task<int> Complete()
        {
            return await Context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}
