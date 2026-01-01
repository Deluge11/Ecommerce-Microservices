using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Interfaces
{
    public interface IIdentityService
    {
        bool VerifyPassword(string password, string hash);
        string GenerateHash(string password);
    }
}
