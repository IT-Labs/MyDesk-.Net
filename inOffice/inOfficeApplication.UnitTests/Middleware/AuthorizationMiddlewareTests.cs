using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System.Net;

namespace inOfficeApplication.UnitTests.Middleware
{
    public class AuthorizationMiddlewareTests
    {
        private IAuthService _authService;

        [OneTimeSetUp]
        public void Setup()
        {
            _authService = Substitute.For<IAuthService>();
        }

        [Test]
        [Order(1)]
        public async Task Invoke_HandleOptionsRequest()
        {
            // Arrange
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware((x) => null);

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = "options";

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            defaultHttpContext.Response.Headers.TryGetValue("Access-Control-Allow-Headers", out StringValues headers);
            Assert.IsTrue(headers.ToString() == "*");

            defaultHttpContext.Response.Headers.TryGetValue("Access-Control-Allow-Origin", out StringValues origin);
            Assert.IsTrue(origin.ToString() == "*");

            defaultHttpContext.Response.Headers.TryGetValue("Access-Control-Allow-Methods", out StringValues methods);
            Assert.IsTrue(methods.ToString() == "GET, POST, PUT, DELETE, OPTIONS");
        }

        [Test]
        [Order(2)]
        public async Task Invoke_AnonimousEndpoint_Success()
        {
            // Arrange
            string parameterName = "test name";
            string parameterValue = "test value";
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware((x) =>
            {
                x.Response.Headers.Add(parameterName, parameterValue);
                return Task.CompletedTask;
            });

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Path = "/register";

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            defaultHttpContext.Response.Headers.TryGetValue(parameterName, out StringValues value);
            Assert.IsTrue(value.ToString() == parameterValue);
        }

        [Test]
        [Order(3)]
        public async Task Invoke_ValidToken_Success()
        {
            // Arrange
            string parameterName = "test name";
            string parameterValue = "test value";
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware((x) =>
            {
                x.Response.Headers.Add(parameterName, parameterValue);
                return Task.CompletedTask;
            });

            string authHeader = "test 1234 test 4321";
            string httpMethod = "POST";
            string url = "/admin/office";

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Request.Method = httpMethod;
            defaultHttpContext.Request.Path = url;
            defaultHttpContext.Request.Headers[HeaderNames.Authorization] = authHeader;

            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Azure).Returns(true);

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            defaultHttpContext.Response.Headers.TryGetValue(parameterName, out StringValues value);
            Assert.IsTrue(value.ToString() == parameterValue);
        }

        [Test]
        [Order(4)]
        public async Task Invoke_HandleSecurityTokenValidationException()
        {
            // Arrange
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware(null);

            string authHeader = "test 1234 test 4321";
            string httpMethod = "POST";
            string url = "/admin/office";
            string exceptionMessage = "exception message";

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Response.Body = new MemoryStream();
            defaultHttpContext.Request.Method = httpMethod;
            defaultHttpContext.Request.Path = url;
            defaultHttpContext.Request.Headers[HeaderNames.Authorization] = authHeader;

            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Azure).Throws(new SecurityTokenValidationException(exceptionMessage));
            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Google).Throws(new SecurityTokenValidationException(exceptionMessage));

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            Assert.IsTrue(defaultHttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized);

            defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(defaultHttpContext.Response.Body);
            string payload = reader.ReadToEnd();

            ErrorDto errorDto = JsonConvert.DeserializeObject<ErrorDto>(payload);
            Assert.NotNull(errorDto);
            Assert.IsTrue(errorDto.Url == url && errorDto.ErrorMessage == exceptionMessage);
        }

        [Test]
        [Order(5)]
        public async Task Invoke_SecurityTokenSignatureKeyNotFoundException_Success()
        {
            // Arrange
            string parameterName = "test name";
            string parameterValue = "test value";
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware((x) =>
            {
                x.Response.Headers.Add(parameterName, parameterValue);
                return Task.CompletedTask;
            });

            string authHeader = "test 1234 test 4321";
            string httpMethod = "GET";
            string url = "/admin/offices";
            string exceptionMessage = "exception message";

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Response.Body = new MemoryStream();
            defaultHttpContext.Request.Method = httpMethod;
            defaultHttpContext.Request.Path = url;
            defaultHttpContext.Request.Headers[HeaderNames.Authorization] = authHeader;

            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Azure).Throws(new SecurityTokenSignatureKeyNotFoundException(exceptionMessage));
            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Google).Throws(new SecurityTokenSignatureKeyNotFoundException(exceptionMessage));
            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Custom).Returns(true);

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            defaultHttpContext.Response.Headers.TryGetValue(parameterName, out StringValues value);
            Assert.IsTrue(value.ToString() == parameterValue);
        }

        [Test]
        [Order(6)]
        public async Task Invoke_SecurityTokenSignatureKeyNotFoundException_Failure()
        {
            // Arrange
            AuthorizationMiddleware authorizationMiddleware = new AuthorizationMiddleware(null);

            string authHeader = "test 1234 test 4321";
            string httpMethod = "PUT";
            string url = "/admin/office-desks";
            string exceptionMessage = "exception message";

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();
            defaultHttpContext.Response.Body = new MemoryStream();
            defaultHttpContext.Request.Method = httpMethod;
            defaultHttpContext.Request.Path = url;
            defaultHttpContext.Request.Headers[HeaderNames.Authorization] = authHeader;

            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Azure).Throws(new SecurityTokenSignatureKeyNotFoundException());
            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Custom).Throws(new SecurityTokenSignatureKeyNotFoundException());
            _authService.ValidateToken(authHeader.Substring(7), url, httpMethod, AuthTypes.Google).Throws(new SecurityTokenSignatureKeyNotFoundException(exceptionMessage));

            // Act
            await authorizationMiddleware.Invoke(defaultHttpContext, _authService);

            // Assert
            Assert.IsTrue(defaultHttpContext.Response.StatusCode == (int)HttpStatusCode.Unauthorized);

            defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(defaultHttpContext.Response.Body);
            string payload = reader.ReadToEnd();

            ErrorDto errorDto = JsonConvert.DeserializeObject<ErrorDto>(payload);
            Assert.NotNull(errorDto);
            Assert.IsTrue(errorDto.Url == url && errorDto.ErrorMessage == exceptionMessage);
        }
    }
}