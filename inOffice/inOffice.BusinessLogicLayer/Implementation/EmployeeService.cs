using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Entities;

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

        public List<CustomEmployee> GetAll()
        {
            List<CustomEmployee> result = new List<CustomEmployee>();

            List<Employee> employees = _employeeRepository.GetAll();

            foreach (Employee employee in employees)
            {
                result.Add(new CustomEmployee(employee.Id, employee.FirstName, employee.LastName, employee.Email, employee.JobTitle));
            }

            return result.DistinctBy(x => x.Email).ToList();
        }

        public Employee GetByEmail(string email)
        {
            return _employeeRepository.GetByEmail(email);
        }

        public Employee GetByEmailAndPassword(string email, string password)
        {
            Employee employee = _employeeRepository.GetByEmail(email);
            if (employee == null)
            {
                return null;
            }

            if (BCrypt.Net.BCrypt.Verify(password, employee.Password))
            {
                return employee;
            }
            else
            {
                return null;
            }
        }
    }
}
