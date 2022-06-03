using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.Repository.Implementation
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Employee Create(Employee employee)
        {
            _context.Add(employee);
            employee.Id = _context.SaveChanges();
            return employee;
        }

        public Employee GetByEmail(string email)
        {
            return _context.Employees.FirstOrDefault(a => a.Email == email);
            
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.ToList();

            
        }

        public Employee GetById(int id)
        {
            return _context.Employees.FirstOrDefault(a => a.Id == id);
        }
    }
}
