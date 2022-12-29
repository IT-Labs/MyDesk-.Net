using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.DTO;
using MyDesk.Data.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace MyDesk.BusinessLogicLayer
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public string GetToken(EmployeeDto employee, string tenant)
        {
            ClaimsIdentity claimsIdentity = new(new[]
            {
                new Claim("id", (employee?.Id??0).ToString()),
                new Claim("name", $"{employee?.FirstName} {employee?.Surname}"),
                new Claim("preferred_username", employee?.Email??string.Empty),
                new Claim("roles", RoleTypes.EMPLOYEE.ToString())
            });

            if (employee?.IsAdmin??false)
            {
                claimsIdentity.AddClaim(new Claim("roles", RoleTypes.ADMIN.ToString()));
            }

            if (!string.IsNullOrEmpty(tenant))
            {
                claimsIdentity.AddClaim(new Claim(_config["TenantClaimKey"], tenant));
            }

            JwtSecurityTokenHandler tokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(_config["Authentication:Local:CustomBearerTokenSigningKey"]);

            SecurityTokenDescriptor tokenDescriptor = new()
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(2),
                Subject = claimsIdentity
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return $"Bearer {tokenHandler.WriteToken(token)}";
        }
    }
}
