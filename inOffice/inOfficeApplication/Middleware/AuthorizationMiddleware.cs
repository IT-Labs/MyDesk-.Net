using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace inOfficeApplication.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly IAuthService _authService;
        private readonly OpenIdConnectConfiguration _openIdConfiguration;

        public AuthorizationMiddleware(RequestDelegate requestDelegate, IOpenIdConnectConfigurationFactory openIdConnectConfigurationFactory, IAuthService authService)
        {
            _requestDelegate = requestDelegate;
            _authService = authService;
            _openIdConfiguration = openIdConnectConfigurationFactory.Create();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.ToUpper() == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                await context.Response.CompleteAsync();
            }
            else
            {
                if (Constants.AnonimousEndpoints.Contains(context.Request.Path) || IsValidToken(context, false, out string message))
                {
                    await _requestDelegate(context);
                }
                else
                {
                    await HandleUnauthorized(context, message);
                }
            }
        }

        private bool IsValidToken(HttpContext context, bool useCustomLogin, out string message)
        {
            try
            {
                message = string.Empty;
                string authHeader = context.Request.Headers[HeaderNames.Authorization];
                string jwtToken = authHeader.Substring(7);

                return _authService.ValidateToken(jwtToken, context.Request.Path.Value, context.Request.Method, useCustomLogin: useCustomLogin, _openIdConfiguration?.SigningKeys);
            }
            catch (SecurityTokenSignatureKeyNotFoundException exception)
            {
                if (useCustomLogin == false)
                {
                    return IsValidToken(context, true, out message);
                }

                message = exception.Message;
                return false;
            }
            catch (SecurityTokenValidationException exception)
            {
                message = exception.Message;
                return false;
            }
        }

        private Task HandleUnauthorized(HttpContext context, string message)
        {
            string result = JsonConvert.SerializeObject(new ErrorDto()
            {
                Url = context.Request.Path,
                ErrorMessage = message
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return context.Response.WriteAsync(result);
        }
    }
}