using MyDesk.Core.DTO;

namespace MyDesk.Core.Interfaces.BusinessLogic
{
    public interface IAuthService
    {
        string GetToken(EmployeeDto employee, string tenant);
    }
}
