using Microsoft.AspNetCore.Http;

namespace inOfficeApplication.Data.Utils
{
    public class Utilities
    {
        public static void GetPaginationParameters(HttpRequest request, out int? take, out int? skip)
        {
            string takeString = request.Query["top"].ToString();
            string skipString = request.Query["skip"].ToString();

            if (int.TryParse(takeString, out int parsedTake) && int.TryParse(skipString, out int parsedSkip))
            {
                take = parsedTake;
                skip = parsedSkip;
            }
            else
            {
                take = null;
                skip = null;
            }
        }
    }
}
