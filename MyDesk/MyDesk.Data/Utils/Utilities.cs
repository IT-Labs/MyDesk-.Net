using Microsoft.AspNetCore.Http;

namespace MyDesk.Data.Utils
{
    public static class Utilities
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

        public static bool IsInRange(this DateTime date, DateTime from, DateTime to)
        {
            return DateTime.Compare(from, date) <= 0 && DateTime.Compare(date, to) <= 0;
        }
    }
}
