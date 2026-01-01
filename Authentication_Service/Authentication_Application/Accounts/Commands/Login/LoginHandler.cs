using Authentication_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Application.Accounts.Commands.Login
{
    public class LoginHandler
    {
        public LoginHandler(
            IUnitOfWork unitOfWork,
            IIdentityService identityService,
            ITokenService tokenService
            )
        {
            UnitOfWork = unitOfWork;
            IdentityService = identityService;
            TokenService = tokenService;
        }

        public IUnitOfWork UnitOfWork { get; }
        public IIdentityService IdentityService { get; }
        public ITokenService TokenService { get; }

        public async Task<string> Handle(LoginCommand request, CancellationToken ct)
        {
            var account = await UnitOfWork.Accounts.Find(e => e.Email == request.email);
            if (account == null)
                return null;

            var isPasswordValid = IdentityService.VerifyPassword(request.password, account.Password);
            if (!isPasswordValid)
                return null;

            var permissions = await UnitOfWork.Permissions.GetByUserId(account.Id);

            return TokenService.GenerateJwtToken(account.Id, permissions);
        }
    }
}
