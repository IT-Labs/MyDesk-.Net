using inOfficeApplication.Data.Entities;

namespace inOfficeApplication.Data.Interfaces.Repository
{
    public interface IEmployeeRepository
    {
        Employee Get(int id);
        Employee GetByEmail(string email);
        List<Employee> GetAll(int? take = null, int? skip = null);
        void Create(Employee employee);
        void Update(Employee employee);
    }
}
