using Authentication_Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Interfaces
{
    public interface IRepositoryPermissions : IRepositoryBase<Permission>
    {
        Task<List<int>> GetByUserId(int userId);
    }
}
