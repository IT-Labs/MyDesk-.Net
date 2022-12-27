using MyDesk.Data.DTO;
using MyDesk.Data.Exceptions;
using MyDesk.Application.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;

namespace MyDesk.UnitTests.Middleware
{
    public class ErrorHandlingMiddlewareTests
    {
        [Test]
        [Order(1)]
        public async Task Invoke_Success()
        {
            // Arrange
            string parameterName = "test name";
            string parameterValue = "test value";
            ErrorHandlingMiddleware errorHandlingMiddleware = new ErrorHandlingMiddleware((x) =>
            {
                x.Response.Headers.Add(parameterName, parameterValue);
                return Task.CompletedTask;
            });

            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();

            // Act
            await errorHandlingMiddleware.Invoke(defaultHttpContext);

            // Assert
            defaultHttpContext.Response.Headers.TryGetValue(parameterName, out StringValues headerValue);
            Assert.IsTrue(headerValue.ToString() == parameterValue);
        }

        [Test]
        [Order(2)]
        public async Task Invoke_HandleNotFoundException()
        {
            // Arrange
            string url = "/NotFoundException";
            string errorMessage = "not found";
            ErrorHandlingMiddleware errorHandlingMiddleware = new ErrorHandlingMiddleware((x) =>
            {
                throw new NotFoundException(errorMessage);
            });

            DefaultHttpContext httpContext = GetDefaultHttpContext(url);

            // Act
            await errorHandlingMiddleware.Invoke(httpContext);

            // Assert
            Assert.IsTrue(httpContext.Response.StatusCode == (int)HttpStatusCode.NotFound);

            string payload = GetPayload(httpContext);

            ErrorDto errorDto = JsonConvert.DeserializeObject<ErrorDto>(payload);
            Assert.NotNull(errorDto);
            Assert.IsTrue(errorDto.Url == url && errorDto.ErrorMessage == errorMessage);
        }

        [Test]
        [Order(3)]
        public async Task Invoke_HandleConflictException()
        {
            // Arrange
            string url = "/ConflictException";
            string errorMessage = "conflict";
            ErrorHandlingMiddleware errorHandlingMiddleware = new ErrorHandlingMiddleware((x) =>
            {
                throw new ConflictException(errorMessage);
            });

            DefaultHttpContext httpContext = GetDefaultHttpContext(url);

            // Act
            await errorHandlingMiddleware.Invoke(httpContext);

            // Assert
            Assert.IsTrue(httpContext.Response.StatusCode == (int)HttpStatusCode.Conflict);

            string payload = GetPayload(httpContext);

            ErrorDto errorDto = JsonConvert.DeserializeObject<ErrorDto>(payload);
            Assert.NotNull(errorDto);
            Assert.IsTrue(errorDto.Url == url && errorDto.ErrorMessage == errorMessage);
        }

        [Test]
        [Order(4)]
        public async Task Invoke_HandleException()
        {
            // Arrange
            string url = "/Exception";
            ErrorHandlingMiddleware errorHandlingMiddleware = new ErrorHandlingMiddleware((x) =>
            {
                throw new Exception("exception message");
            });

            DefaultHttpContext httpContext = GetDefaultHttpContext(url);

            // Act
            await errorHandlingMiddleware.Invoke(httpContext);

            // Assert
            Assert.IsTrue(httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError);

            string payload = GetPayload(httpContext);

            ErrorDto errorDto = JsonConvert.DeserializeObject<ErrorDto>(payload);
            Assert.NotNull(errorDto);
            Assert.IsTrue(errorDto.Url == url);
        }

        #region Private methods
        private DefaultHttpContext GetDefaultHttpContext(string url)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            httpContext.Request.Path = new PathString(url);

            return httpContext;
        }

        private string GetPayload(DefaultHttpContext httpContext)
        {
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(httpContext.Response.Body);
            string text = reader.ReadToEnd();

            return text;
        }
        #endregion
    }
}
