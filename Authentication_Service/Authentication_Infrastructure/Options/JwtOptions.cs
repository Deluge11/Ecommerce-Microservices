using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Infrastructure.Options
{
    public class JwtOptions
    {
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int LifeTime { get; init; }
        public string SigningKey { get; init; }
    }
}
