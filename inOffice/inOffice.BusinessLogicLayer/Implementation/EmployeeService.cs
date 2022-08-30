using AutoMapper;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;

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

        public GenericResponse SetEmployeeAsAdmin(int id)
        {
            Employee employee = _employeeRepository.Get(id);

            if (employee == null)
            {
                return new GenericResponse() { Success = false };
            }

            employee.IsAdmin = true;
            _employeeRepository.Update(employee);

            return new GenericResponse() { Success = true };
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
