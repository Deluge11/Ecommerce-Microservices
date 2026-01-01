

using Authentication_Core.Interfaces;

namespace Authentication_Infrastructure.Security
{
    public class IdentityService : IIdentityService
    {
        private readonly Microsoft.AspNetCore.Identity.PasswordHasher<object> _passwordHasher =
            new Microsoft.AspNetCore.Identity.PasswordHasher<object>();

        public string GenerateHash(string password)
        {
            return _passwordHasher.HashPassword(new object(), password);
        }

        public bool VerifyPassword(string hash, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(new object(), hash, password);
            return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
        }
    }
}