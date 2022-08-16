using inOfficeApplication.Data.Models;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        Employee GetByEmail(string email);
        Employee GetById(int id);
        List<Employee> GetAll(int? take = null, int? skip = null);
        void Create(Employee employee);
    }
}
