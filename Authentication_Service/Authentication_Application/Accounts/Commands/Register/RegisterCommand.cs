using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Application.Accounts.Commands.Register
{
    public record RegisterCommand(string name, string email, string password,string confirmPassword) : IRequest<string>;
}
