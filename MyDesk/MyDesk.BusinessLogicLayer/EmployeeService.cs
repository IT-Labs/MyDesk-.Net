using AutoMapper;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Interfaces.Repository;
using MyDesk.Data.DTO;
using MyDesk.Data.Entities;
using MyDesk.Data.Exceptions;

namespace MyDesk.BusinessLogicLayer
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
            var employee = _employeeRepository.Get(id);

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

        public EmployeeDto? GetByEmail(string email)
        {
            var employee = _employeeRepository.GetByEmail(email);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public EmployeeDto GetByEmailAndPassword(string email, string password)
        {
            var employee = _employeeRepository.GetByEmail(email);

            if (employee == null || !BCrypt.Net.BCrypt.Verify(password, employee.Password))
            {
                throw new NotFoundException($"Employee with email: {email} not found.");
            }

            return _mapper.Map<EmployeeDto>(employee);
        }
        public void UpdateEmployee(EmployeeDto employeeDto)
        {
            if (employeeDto.Id == null)
            {
                throw new NotFoundException($"Employee Id is not provided");
            }

            var employee = _employeeRepository.Get(employeeDto.Id.Value);

            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID: {employeeDto.Id} not found.");
            }

            if (employeeDto.Email != null && employeeDto.Email != employee.Email && (employee.IsSSOAccount ?? false))
            {
                throw new NotFoundException($"Cannot change email for a SSO Account.");
            }

            if (employeeDto.IsSSOAccount != null && employeeDto.IsSSOAccount != employee.IsSSOAccount)
            {
                throw new NotFoundException($"Cannot reconfigure IsSSOAccount flag.");
            }

            if (employeeDto.Email != null)
            {
                var existingEmployeee = _employeeRepository.GetByEmail(employeeDto.Email ?? String.Empty);

                if (existingEmployeee != null && existingEmployeee.Id != employeeDto?.Id)
                {
                    throw new ConflictException("There is already and employee with same email.");
                }
            }

            if (employeeDto.JobTitle != null)
                employee.JobTitle = employeeDto.JobTitle;
            if (employeeDto.Email != null)
                employee.Email = employeeDto.Email;
            if (employeeDto.FirstName != null)
                employee.FirstName = employeeDto.FirstName;
            if (employeeDto.Surname != null)
                employee.LastName = employeeDto.Surname;
            if (employeeDto.IsAdmin != null)
                employee.IsAdmin = employeeDto.IsAdmin;

            _employeeRepository.Update(employee);
        }

    }
}
