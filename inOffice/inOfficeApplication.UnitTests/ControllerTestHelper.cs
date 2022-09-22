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
            return "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjExMSIsIm5hbWUiOiJKb2huIERvZSIsInByZWZlcnJlZF91c2VybmFtZSI6InRlc3RAaXQtbGFicy5jb20iLCJyb2xlcyI6IkVNUExPWUVFIiwibmJmIjoxNjYzNzcxMTE3LCJleHAiOjE2NjM3NzgzMTYsImlhdCI6MTY2Mzc3MTExNywiaXNzIjoiaHR0cHM6Ly9sb2dpbi5taWNyb3NvZnRvbmxpbmUuY29tLzlhNDMzNjExLTBjODEtNGY3Yi1hYmFlLTg5MTM2NGRkZGExNy92Mi4wIiwiYXVkIjoiNDMxYzVkMjEtMTNkMS00M2FmLWEzYmMtNjU0ODRhMGJjYTI5In0.P-EVq2BrUSDNxubfIYwteKDc2U6TY4CUD5xFs2hAGpE";
        }
    }
}
