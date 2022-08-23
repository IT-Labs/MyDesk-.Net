using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface IEmployeeRepository
    {
        void Create(Employee employee);
        Employee GetByEmail(string email);
        List<Employee> GetAll();
    }
}
