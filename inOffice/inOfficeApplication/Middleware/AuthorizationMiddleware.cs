using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace inOfficeApplication.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IConfiguration _configuration;
        private readonly OpenIdConnectConfiguration _openIdConfiguration;

        public AuthorizationMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
        {
            _requestDelegate = requestDelegate;
            _configuration = configuration;

            if (!_configuration.GetValue<bool>("Settings:UseCustomBearerToken"))
            {
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(_configuration["Settings:MetadataAddress"], new OpenIdConnectConfigurationRetriever());
                _openIdConfiguration = configurationManager.GetConfigurationAsync(CancellationToken.None).Result;
            }
        }

        public async Task Invoke(HttpContext context)
        {
            if (Constants.AnonimousEndpoints.Contains(context.Request.Path) || IsValidToken(context, out string message))
            {
                await _requestDelegate(context);
            }
            else
            {
                await HandleUnauthorized(context, message);
            }
        }

        private bool IsValidToken(HttpContext context, out string message)
        {
            try
            {
                message = string.Empty;
                string authHeader = context.Request.Headers[HeaderNames.Authorization];
                string jwtToken = authHeader.Substring(7);

                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                tokenHandler.ValidateToken(jwtToken, GetTokenValidationParameters(), out SecurityToken validatedToken);

                return HasRoles(context, validatedToken);
            }
            catch (SecurityTokenValidationException exception)
            {
                message = exception.Message;
                return false;
            }
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            TokenValidationParameters parameters;

            if (_configuration.GetValue<bool>("Settings:UseCustomBearerToken"))
            {
                byte[] key = Encoding.ASCII.GetBytes(_configuration["Settings:CustomBearerTokenSigningKey"]);
                parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["JwtInfo:Issuer"],
                    ValidAudience = _configuration["JwtInfo:Audience"]
                };
            }
            else
            {
                parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = _openIdConfiguration.SigningKeys,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = _configuration["JwtInfo:Issuer"],
                    ValidAudience = _configuration["JwtInfo:Audience"]
                };
            }

            return parameters;
        }

        private bool HasRoles(HttpContext context, SecurityToken securityToken)
        {
            RoleTypes requiredRole = GetRequiredRole(context);

            JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)securityToken;
            if (_configuration.GetValue<bool>("Settings:UseCustomBearerToken"))
            {
                bool isAdmin = bool.Parse(jwtSecurityToken.Claims.First(x => x.Type == "IsAdmin").Value);
                if (isAdmin)
                {
                    return true;
                }
                else
                {
                    return requiredRole == RoleTypes.All || requiredRole == RoleTypes.Employee;
                }
            }
            else
            {
                List<Claim> roleClaims = jwtSecurityToken.Claims.Where(x => x.Type == "roles").ToList();
                if (requiredRole == RoleTypes.Admin)
                {
                    return roleClaims.Any(x => x.Value == "ADMIN");
                }
                else if (requiredRole == RoleTypes.Employee)
                {
                    return roleClaims.Any(x => x.Value == "EMPLOYEE");
                }
                else
                {
                    return roleClaims.Any();
                }
            }
        }

        private RoleTypes GetRequiredRole(HttpContext context)
        {
            string path = context.Request.Path.Value;
            string pathId = Regex.Match(path, @"\d+").Value;

            if (!string.IsNullOrEmpty(pathId))
            {
                path = path.Replace(pathId, "{id}");
            }

            Tuple<string, string> roles = Tuple.Create(context.Request.Method.ToUpper(), path.ToLower());
            if (Constants.AdminEndpoints.Contains(roles))
            {
                return RoleTypes.Admin;
            }
            else if (Constants.EmployeeEndpoints.Contains(roles))
            {
                return RoleTypes.Employee;
            }
            else if (Constants.AllEndpoints.Contains(roles))
            {
                return RoleTypes.All;
            }
            else
            {
                throw new Exception("Required endpoint doesn't exist");
            }
        }

        private Task HandleUnauthorized(HttpContext context, string message)
        {
            string result = JsonConvert.SerializeObject(new
            {
                url = context.Request.Path,
                error = message
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return context.Response.WriteAsync(result);
        }
    }
}
