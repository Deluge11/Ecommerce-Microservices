using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Models;
using Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Business
{
    public class InventoryKeyGenerator
    {
        public InventoryKeyGenerator(InventoryOptions inventoryOptions, ILogger<InventoryKeyGenerator> logger)
        {
            InventoryOptions = inventoryOptions;
            Logger = logger;
        }

        public InventoryOptions InventoryOptions { get; }
        public ILogger Logger { get; }

        public string GenerateJwt()
        {
            if(InventoryOptions == null)
            {
                Logger.LogWarning("Inventory Options Equal Null");
                return string.Empty;
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(InventoryOptions.Key));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: InventoryOptions.Issuer,
                audience: InventoryOptions.Audience,
                expires: DateTime.UtcNow.AddMinutes(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
