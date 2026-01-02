using Authentication_Application.Accounts.Commands.Login;
using Authentication_Core.Entities;
using Authentication_Core.Enums;
using Authentication_Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication_Application.Accounts.Commands.Register
{
    public class RegisterHandler
    {
        public RegisterHandler(
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

        public async Task<string> Handle(RegisterCommand request, CancellationToken ct)
        {
            if (request.password != request.confirmPassword)
                return null;

            var accountExists = await UnitOfWork.Accounts.Exists(e => e.Email == request.email);
            if (accountExists)
                return null;

            var hashedPassword = IdentityService.GenerateHash(request.password);
            var defaultRole = await UnitOfWork.Roles.Find(r => r.Id == (int)ERole.Customer);

            var newAccount = new Account
            {
                Name = request.name,
                Email = request.email,
                Password = hashedPassword,
                Roles = new List<Role> { defaultRole }
            };

            await UnitOfWork.Accounts.Add(newAccount);

            if (await UnitOfWork.Complete() == 0)
                return null;

            //Publish New User Message

            var permissions = await UnitOfWork.Permissions.GetByUserId(newAccount.Id);

            return TokenService.GenerateJwtToken(newAccount.Id, permissions);
        }
    }
}
