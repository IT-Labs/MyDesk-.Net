using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace inOfficeApplication.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private IAuthService _authService;

        public AuthorizationMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            _authService = authService;

            if (context.Request.Method.ToUpper() == "OPTIONS")
            {
                context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
                context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                await context.Response.CompleteAsync();
            }
            else
            {
                if (Constants.AnonimousEndpoints.Contains(context.Request.Path) || IsValidToken(context, AuthTypes.Azure, out string message))
                {
                    await _requestDelegate(context);
                }
                else
                {
                    await HandleUnauthorized(context, message);
                }
            }
        }

        private bool IsValidToken(HttpContext context, AuthTypes authType, out string message)
        {
            try
            {
                message = string.Empty;
                string authHeader = context.Request.Headers[HeaderNames.Authorization];
                string jwtToken = authHeader.Substring(7);

                return _authService.ValidateToken(jwtToken, context.Request.Path.Value, context.Request.Method, authType);
            }
            catch (SecurityTokenValidationException exception)
            {
                if (exception is SecurityTokenSignatureKeyNotFoundException || exception is SecurityTokenUnableToValidateException)
                {
                    if (authType == AuthTypes.Azure)
                    {
                        return IsValidToken(context, AuthTypes.Custom, out message);
                    }
                    else if (authType == AuthTypes.Custom)
                    {
                        return IsValidToken(context, AuthTypes.Google, out message);
                    }
                }

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