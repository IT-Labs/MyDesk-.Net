using MyDesk.Data.DTO;
using MyDesk.Data.Utils;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IAuthService
    {
        string GetToken(EmployeeDto employee, string tenant);
    }
}
