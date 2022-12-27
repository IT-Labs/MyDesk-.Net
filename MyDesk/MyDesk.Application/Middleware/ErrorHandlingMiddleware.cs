using MyDesk.Data.DTO;
using MyDesk.Data.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace MyDesk.Application.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;

        public ErrorHandlingMiddleware(RequestDelegate requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception exception)
            {
                await HandleException(context, exception);
            }
        }

        private Task HandleException(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string errorMessage;

            if (exception is NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
                errorMessage = exception.Message;
            }
            else if (exception is ConflictException)
            {
                statusCode = HttpStatusCode.Conflict;
                errorMessage = exception.Message;
            }
            else
            {
                statusCode = HttpStatusCode.InternalServerError;
                errorMessage = exception.ToString();
            }

            string result = JsonConvert.SerializeObject(new ErrorDto()
            {
                Url = context.Request.Path,
                ErrorMessage = errorMessage
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
