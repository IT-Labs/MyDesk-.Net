using System.Collections.ObjectModel;

namespace inOfficeApplication.Data.Utils
{
    public class Constants
    {
        public static readonly IList<string> AnonimousEndpoints = new ReadOnlyCollection<string> (new List<string>() { "/token" });
        public static readonly IList<Tuple<string, string>> AllEndpoints = new ReadOnlyCollection<Tuple<string, string>>(new List<Tuple<string, string>>() 
        {
            Tuple.Create("POST", "/authentication"),
            Tuple.Create("POST", "/register"),
            Tuple.Create("GET", "/employee/reservations/all"),
            Tuple.Create("GET", "/employee/reviews/all"),
            Tuple.Create("GET", "/employee/offices"),
            Tuple.Create("GET", "/employee/office/image/{id}"),
            Tuple.Create("POST", "/employee/reserve"),
            Tuple.Create("POST", "/employee/reserve/coworker"),
            Tuple.Create("GET", "/employee/all"),
            Tuple.Create("GET", "/employee/future-reservation"),
            Tuple.Create("GET", "/employee/past-reservations"),
            Tuple.Create("GET", "/employee/review/{id}"),
            Tuple.Create("POST", "/employee/review"),
            Tuple.Create("DELETE", "/employee/reserve/{id}")
        });
        public static readonly IList<Tuple<string, string>> AdminEndpoints = new ReadOnlyCollection<Tuple<string, string>>(new List<Tuple<string, string>>()
        {
            Tuple.Create("GET", "/entity/reviews/{id}"),
            Tuple.Create("POST", "/admin/office-entities/{id}"),
            Tuple.Create("GET", "/admin/office-desks/{id}"),
            Tuple.Create("DELETE", "/admin/entity"),
            Tuple.Create("PUT", "/admin/office-entities"),
            Tuple.Create("GET", "/admin/office-conferencerooms/{id}"),
            Tuple.Create("POST", "/admin/office"),
            Tuple.Create("PUT", "/admin/office/{id}"),
            Tuple.Create("DELETE", "/admin/office/{id}")
        });
        public static readonly IList<Tuple<string, string>> EmployeeEndpoints = new ReadOnlyCollection<Tuple<string, string>>(new List<Tuple<string, string>>()
        {
            Tuple.Create("GET", "/admin/office/image/{id}"),
            Tuple.Create("GET", "/employee/office-conferencerooms/{id}"),
            Tuple.Create("GET", "/employee/office-desks/{id}"),
            Tuple.Create("GET", "/admin/offices")
        });
    }

    public enum RoleTypes
    {
        Admin,
        Employee,
        All
    }
}
