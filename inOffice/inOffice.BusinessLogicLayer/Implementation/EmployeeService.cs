using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
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

        public Employee GetById(int id)
        {
            return _employeeRepository.GetById(id);
        }
    }
}
