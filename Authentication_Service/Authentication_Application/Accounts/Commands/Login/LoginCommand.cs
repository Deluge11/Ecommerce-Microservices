using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Application.Accounts.Commands.Login
{
    public record LoginCommand(string email, string password) : IRequest<string>;
}
