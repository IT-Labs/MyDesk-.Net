using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;

namespace inOffice.BusinessLogicLayer.Implementation
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public void Create(Employee employee)
        {
            _employeeRepository.Create(employee);
        }

        public void SetEmployeeAsAdmin(int id)
        {
            Employee employee = _employeeRepository.Get(id);

            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID:{id} not found.");
            }

            employee.IsAdmin = true;
            _employeeRepository.Update(employee);
        }

        public List<EmployeeDto> GetAll(int? take = null, int? skip = null)
        {
            List<Employee> employees = _employeeRepository.GetAll(take: take, skip: skip);
            List<EmployeeDto> result = _mapper.Map<List<EmployeeDto>>(employees);

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

            return BCrypt.Net.BCrypt.Verify(password, employee.Password) ? employee : null;
        }
    }
}
