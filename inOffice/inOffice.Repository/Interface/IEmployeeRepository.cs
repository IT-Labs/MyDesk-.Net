using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IEmployeeRepository
    {
        void Create(Employee employee);
        Employee GetByEmail(string email);
        Employee GetById(int id);
        List<Employee> GetAll();
    }
}
