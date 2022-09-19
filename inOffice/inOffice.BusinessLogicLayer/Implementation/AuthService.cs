using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IApplicationParmeters _applicationParmeters;
        public AuthService(IApplicationParmeters applicationParmeters)
        {
            _applicationParmeters = applicationParmeters;
        }

        public string GetToken(EmployeeDto employee)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("id", employee.Id.ToString()),
                new Claim("name", $"{employee.FirstName} {employee.LastName}"),
                new Claim("preferred_username", employee.Email),
                new Claim("roles", RoleTypes.EMPLOYEE.ToString())
            });

            if (employee.IsAdmin == true)
            {
                claimsIdentity.AddClaim(new Claim("roles", RoleTypes.ADMIN.ToString()));
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_applicationParmeters.GetCustomBearerTokenSigningKey());
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _applicationParmeters.GetJwtIssuer(),
                Audience = _applicationParmeters.GetJwtAudience(),
                Subject = claimsIdentity
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return $"Bearer {tokenHandler.WriteToken(token)}";
        }

        public bool ValidateToken(string jwtToken, string url, string httpMethod, bool useCustomLogin, ICollection<SecurityKey> signingKeys)
        {
            new JwtSecurityTokenHandler().ValidateToken(jwtToken, GetTokenValidationParameters(useCustomLogin, signingKeys), out SecurityToken validatedToken);
            return HasRoles(url, httpMethod, validatedToken);
        }

        #region Private methods
        private TokenValidationParameters GetTokenValidationParameters(bool useCustomLogin, ICollection<SecurityKey> signingKeys)
        {
            TokenValidationParameters parameters;

            if (useCustomLogin)
            {
                byte[] key = Encoding.ASCII.GetBytes(_applicationParmeters.GetCustomBearerTokenSigningKey());
                parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _applicationParmeters.GetJwtIssuer(),
                    ValidAudience = _applicationParmeters.GetJwtAudience()
                };
            }
            else
            {
                parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = signingKeys,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _applicationParmeters.GetJwtIssuer(),
                    ValidAudience = _applicationParmeters.GetJwtAudience()
                };
            }

            return parameters;
        }

        private bool HasRoles(string url, string httpMethod, SecurityToken securityToken)
        {
            RoleTypes requiredRole = GetRequiredRole(url, httpMethod);

            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)securityToken;
            List<Claim> roleClaims = jwtSecurityToken.Claims.Where(x => x.Type == "roles").ToList();
            if (requiredRole == RoleTypes.ADMIN)
            {
                return roleClaims.Any(x => x.Value == RoleTypes.ADMIN.ToString());
            }
            else if (requiredRole == RoleTypes.EMPLOYEE)
            {
                return roleClaims.Any(x => x.Value == RoleTypes.EMPLOYEE.ToString());
            }
            else
            {
                return roleClaims.Any();
            }
        }

        private RoleTypes GetRequiredRole(string url, string httpMethod)
        {
            string path = url.TrimEnd('/');
            string pathId = Regex.Match(path, @"\d+").Value;

            if (!string.IsNullOrEmpty(pathId))
            {
                path = path.Replace(pathId, "{id}");
            }

            Tuple<string, string> roles = Tuple.Create(httpMethod.ToUpper(), path.ToLower());
            if (Constants.AdminEndpoints.Contains(roles))
            {
                return RoleTypes.ADMIN;
            }
            else if (Constants.EmployeeEndpoints.Contains(roles))
            {
                return RoleTypes.EMPLOYEE;
            }
            else if (Constants.AllEndpoints.Contains(roles))
            {
                return RoleTypes.ALL;
            }
            else
            {
                throw new Exception("Required endpoint doesn't exist.");
            }
        }
        #endregion
    }
}
