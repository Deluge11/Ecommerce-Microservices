using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, List<int> permissions);
    }
}
