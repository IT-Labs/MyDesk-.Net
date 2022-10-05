using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IAuthService
    {
        string GetToken(EmployeeDto employeeDto);
        bool ValidateToken(string jwtToken, string url, string httpMethod, AuthTypes authType);
    }
}
