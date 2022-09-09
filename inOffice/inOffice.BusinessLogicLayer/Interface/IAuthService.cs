using inOfficeApplication.Data.DTO;
using Microsoft.IdentityModel.Tokens;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IAuthService
    {
        string GetToken(EmployeeDto employeeDto);
        bool ValidateToken(string jwtToken, string url, string httpMethod, ICollection<SecurityKey> signingKeys);
    }
}
