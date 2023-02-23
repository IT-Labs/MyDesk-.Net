using AutoMapper;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Entities;
using MyDesk.Core.Exceptions;
using MyDesk.Core.Database;

namespace MyDesk.BusinessLogicLayer
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper _mapper;
        private readonly IContext _context;

        public EmployeeService(IMapper mapper, IContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public void Create(EmployeeDto employeeDto)
        {
            Employee employee = _mapper.Map<Employee>(employeeDto);
            _context.Insert(employee);
        }

        public void SetEmployeeAsAdmin(int id)
        {
           var employee = GetEmployeeById(id);
            if (employee == null)
            {
                throw new NotFoundException($"Employee with ID:{id} not found.");
            }

            employee.IsAdmin = true;
            _context.Modify(employee);
        }

        public List<EmployeeDto> GetAll(int? take = null, int? skip = null)
        {
            var query = _context
                .AsQueryable<Employee>()
                .Where(x => x.IsDeleted == false)
                .DistinctBy(x => x.Email);

            var employees = (take.HasValue && skip.HasValue) ?
                query.Skip(skip.Value).Take(take.Value).ToList() :
                query.ToList();

            var result = _mapper.Map<List<EmployeeDto>>(employees);
            return result;
        }

        public EmployeeDto? GetByEmail(string email)
        {
            var employee = GetEmployeeByEmail(email);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public EmployeeDto GetByEmailAndPassword(string email, string password)
        {
            var employee = GetEmployeeByEmail(email);
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

            var employee = GetEmployeeById(employeeDto.Id.Value);

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

            if (!string.IsNullOrWhiteSpace(employeeDto.Email))
            {
                var existingEmployeee = GetEmployeeByEmail(employeeDto.Email);
                if (existingEmployeee != null && existingEmployeee.Id != employeeDto?.Id)
                {
                    throw new ConflictException("There is already and employee with same email.");
                }
            }

            if (!string.IsNullOrWhiteSpace(employeeDto.JobTitle))
                employee.JobTitle = employeeDto.JobTitle;
            if (!string.IsNullOrWhiteSpace(employeeDto.Email))
                employee.Email = employeeDto.Email;
            if (!string.IsNullOrWhiteSpace(employeeDto.FirstName))
                employee.FirstName = employeeDto.FirstName;
            if (!string.IsNullOrWhiteSpace(employeeDto.Surname))
                employee.LastName = employeeDto.Surname;
            if (employeeDto.IsAdmin != null)
                employee.IsAdmin = employeeDto.IsAdmin;

            _context.Modify(employee);
        }

        private Employee? GetEmployeeByEmail(string email)
        {
            return _context
                .AsQueryable<Employee>()
                .FirstOrDefault(x => x.Email.ToLower() == email.ToLower() && x.IsDeleted == false);
        }

        private Employee? GetEmployeeById(int id)
        {
            return _context
                .AsQueryable<Employee>()
                .FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
        }
    }
}
