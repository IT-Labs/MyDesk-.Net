using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.DTO;
using MyDesk.Data.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MyDesk.BusinessLogicLayer
{
    public class AuthService : IAuthService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IApplicationParmeters _applicationParmeters;

        public AuthService(IApplicationParmeters applicationParmeters, IWebHostEnvironment environment)
        {
            _applicationParmeters = applicationParmeters;
            _environment = environment;
        }

        public string GetToken(EmployeeDto employee, string tenant)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("id", employee.Id.ToString()),
                new Claim("name", $"{employee.FirstName} {employee.Surname}"),
                new Claim("preferred_username", employee.Email),
                new Claim("roles", RoleTypes.EMPLOYEE.ToString())
            });

            if (employee.IsAdmin == true)
            {
                claimsIdentity.AddClaim(new Claim("roles", RoleTypes.ADMIN.ToString()));
            }

            if (!string.IsNullOrEmpty(tenant))
            {
                claimsIdentity.AddClaim(new Claim(_applicationParmeters.GetTenantClaimKey(), tenant));
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_applicationParmeters.GetCustomBearerTokenSigningKey(_environment.IsDevelopment()));

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
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
