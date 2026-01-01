using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
