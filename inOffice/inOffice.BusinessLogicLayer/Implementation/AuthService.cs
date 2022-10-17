using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Http;
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
        private readonly IOpenIdConfigurationKeysFactory _openIdConfigurationKeysFactory;
        private readonly Func<IEmployeeRepository> _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IApplicationParmeters applicationParmeters,
            IOpenIdConfigurationKeysFactory openIdConfigurationKeysFactory,
            Func<IEmployeeRepository> employeeRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _applicationParmeters = applicationParmeters;
            _openIdConfigurationKeysFactory = openIdConfigurationKeysFactory;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
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

        public bool ValidateToken(string jwtToken, string url, string httpMethod, AuthTypes authType)
        {
            IEnumerable<SecurityKey> securityKeys = _openIdConfigurationKeysFactory.GetKeys(authType);

            new JwtSecurityTokenHandler().ValidateToken(jwtToken, GetTokenValidationParameters(authType, securityKeys), out SecurityToken validatedToken);

            string email = string.Empty;
            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)validatedToken;

            if (authType == AuthTypes.Google)
            {
                email = jwtSecurityToken.Claims.First(x => x.Type == "email").Value;
            }
            else
            {
                email = jwtSecurityToken.Claims.First(x => x.Type == "preferred_username").Value;
            }

            Claim? tenantClaim = jwtSecurityToken.Claims.SingleOrDefault(x => x.Type == _applicationParmeters.GetTenantClaimKey());
            if (tenantClaim != null)
            {
                string tenantName = tenantClaim.Value;
                if (!string.IsNullOrEmpty(tenantName))
                {
                    Dictionary<string, string> tenants = _applicationParmeters.GetTenants();

                    if (tenants.ContainsKey(tenantName))
                    {
                        _httpContextAccessor.HttpContext.Items["tenant"] = tenants[tenantName];
                    }
                }
            }

            Employee employee = _employeeRepository().GetByEmail(email);
            if (employee == null)
            {
                throw new SecurityTokenValidationException($"Employee with email {email} was not found in DB.");
            }

            return HasRoles(url, httpMethod, jwtSecurityToken, employee, authType);
        }

        #region Private methods
        private TokenValidationParameters GetTokenValidationParameters(AuthTypes authType, IEnumerable<SecurityKey> securityKeys)
        {
            TokenValidationParameters parameters = null;

            if (authType == AuthTypes.Custom)
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
            else if (authType == AuthTypes.Azure || authType == AuthTypes.Google)
            {
                parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = securityKeys,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _applicationParmeters.GetJwtIssuer(),
                    ValidAudience = _applicationParmeters.GetJwtAudience()
                };
            }

            return parameters;
        }

        private bool HasRoles(string url, string httpMethod, JwtSecurityToken jwtSecurityToken, Employee employee, AuthTypes authType)
        {
            RoleTypes requiredRole = GetRequiredRole(url, httpMethod);

            if (authType == AuthTypes.Google)
            {
                if (requiredRole == RoleTypes.ADMIN)
                {
                    return employee.IsAdmin.Value;
                }

                return true;
            }
            else
            {
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
                throw new NotFoundException("Required endpoint doesn't exist.");
            }
        }
        #endregion
    }
}
