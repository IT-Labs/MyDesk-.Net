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

        public Employee Get(int id)
        {
            return _context.Employees.FirstOrDefault(x => x.Id == id && x.IsDeleted == false);
        }

        public Employee GetByEmail(string email)
        {
            return _context.Employees.FirstOrDefault(a => a.Email == email && a.IsDeleted == false);
        }

        public List<Employee> GetAll(int? take = null, int? skip = null)
        {
            IQueryable<Employee> query = _context.Employees.Where(x => x.IsDeleted == false);

            if (take.HasValue && skip.HasValue)
            {
                query = query.Skip(skip.Value).Take(take.Value);
            }

            return query.ToList();
        }

        public void Create(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
        }

        public void Update(Employee employee)
        {
            _context.Employees.Update(employee);
            _context.SaveChanges();
        }
    }
}
