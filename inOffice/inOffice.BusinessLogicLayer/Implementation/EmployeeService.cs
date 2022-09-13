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

        public void Create(EmployeeDto employeeDto)
        {
            Employee employee = _mapper.Map<Employee>(employeeDto);
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

        public EmployeeDto GetByEmail(string email)
        {
            Employee employee = _employeeRepository.GetByEmail(email);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public EmployeeDto GetByEmailAndPassword(string email, string password)
        {
            Employee employee = _employeeRepository.GetByEmail(email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.Password))
            {
                throw new NotFoundException($"Employee with email: {email} not found.");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }
    }
}
