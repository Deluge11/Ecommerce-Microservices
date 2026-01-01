using Authentication_Core.Entities;
using Authentication_Core.Interfaces;
using Authentication_Core.Repositories;
using Authentication_Infrastructure.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Infrastructure.Repositories
{
    public class RepositoryPermission : RepositoryBase<Permission>, IRepositoryPermissions
    {

        public RepositoryPermission(AppDbContext context) : base(context)
        {
            Context = context;
        }

        public AppDbContext Context { get; }

        public async Task<List<int>> GetByUserId(int userId)
        {
            var permissionIds =
            await Context.Accounts
           .Where(a => a.Id == userId)
           .SelectMany(a => a.Roles)
           .SelectMany(r => r.Permissions)
           .Select(p => p.Id)
           .Distinct()
           .ToListAsync();

            return permissionIds;
        }


    }
}
