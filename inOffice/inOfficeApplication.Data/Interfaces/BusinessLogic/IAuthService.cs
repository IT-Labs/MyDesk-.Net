using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IAuthService
    {
        string GetToken(EmployeeDto employee, string tenant);
    }
}
