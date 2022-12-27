using MyDesk.Data.DTO;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IEmployeeService
    {
        EmployeeDto GetByEmail(string email);
        EmployeeDto GetByEmailAndPassword(string email, string password);
        List<EmployeeDto> GetAll(int? take = null, int? skip = null);
        void Create(EmployeeDto employeeDto);
        void SetEmployeeAsAdmin(int id);
    }
}
