using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public void Create(Employee employee)
        {
            _employeeRepository.Create(employee);
        }

        public List<Employee> GetAll(int? take = null, int? skip = null)
        {
            return _employeeRepository.GetAll();
        }

        public Employee GetByEmail(string email)
        {
            return _employeeRepository.GetByEmail(email);
        }

        public Employee GetById(int id)
        {
            return _employeeRepository.GetById(id);
        }
    }
}
