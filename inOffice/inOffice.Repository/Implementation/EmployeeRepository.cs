using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Create(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public Employee GetByEmail(string email)
        {
            return _context.Employees.FirstOrDefault(a => a.Email == email && a.IsDeleted == false);
        }

        public List<Employee> GetAll()
        {
            return _context.Employees.Where(x => x.IsDeleted == false).ToList();
        }
    }
}
