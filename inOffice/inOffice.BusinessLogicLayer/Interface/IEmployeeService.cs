using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        Employee GetByEmail(string email);
        Employee GetByEmailAndPassword(string email, string password);
        List<EmployeeDto> GetAll(int? take = null, int? skip = null);
        void Create(Employee employee);
        GenericResponse SetEmployeeAsAdmin(int id);
    }
}
