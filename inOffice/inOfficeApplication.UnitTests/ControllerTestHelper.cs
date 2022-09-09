using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.UnitTests
{
    public class ControllerTestHelper
    {
        public static DefaultHttpContext GetMockedHttpContext(int? take = null, int? skip = null)
        {
            DefaultHttpContext httpContext = new DefaultHttpContext();

            if (take.HasValue && skip.HasValue)
            {
                Dictionary<string, StringValues> dictionary = new Dictionary<string, StringValues>()
                {
                    { "top", new StringValues(take.Value.ToString()) },
                    { "skip", new StringValues(skip.Value.ToString()) }
                };

                IQueryCollection queryParams = new QueryCollection(dictionary);
                httpContext.Request.Query = queryParams;
            }

            httpContext.Request.Headers[HeaderNames.Authorization] = GetToken();

            return httpContext;
        }

        private static string GetToken()
        {
            return "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjcyIiwibmFtZSI6IkFsZWtzYW5kYXIgRGp1cm92c2tpIiwicHJlZmVycmVkX3VzZXJuYW1lIjoiYWxla3NhbmRhci5kanVyb3Zza2lAaXQtbGFicy5jb20iLCJyb2xlcyI6IkVNUExPWUVFIiwibmJmIjoxNjYyNzExODE3LCJleHAiOjE2NjI3MTkwMTcsImlhdCI6MTY2MjcxMTgxNywiaXNzIjoiaHR0cHM6Ly9sb2dpbi5taWNyb3NvZnRvbmxpbmUuY29tLzlhNDMzNjExLTBjODEtNGY3Yi1hYmFlLTg5MTM2NGRkZGExNy92Mi4wIiwiYXVkIjoiNDMxYzVkMjEtMTNkMS00M2FmLWEzYmMtNjU0ODRhMGJjYTI5In0.ZVhKXQOH0h7dn6LYEoVggaF1GT5628lLBoTKgD0T6WA";
        }
    }
}
