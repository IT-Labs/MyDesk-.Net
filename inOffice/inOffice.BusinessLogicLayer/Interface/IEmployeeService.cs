using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
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
