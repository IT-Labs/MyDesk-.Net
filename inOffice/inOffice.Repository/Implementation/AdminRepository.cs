using inOffice.Repository.Interface;
using inOfficeApplication.Data;
using inOfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.Repository.Implementation
{
    public class AdminRepository : IAdminRepository
    {

        private readonly ApplicationDbContext _context;

        public AdminRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public Admin Create(Admin admin)
        {
            _context.Add(admin);
            admin.Id = _context.SaveChanges(); 
            return admin;
        }

        public Admin GetByEmail(string email)
        {
            return _context.Admins.FirstOrDefault(a => a.Email == email);
        }

        public Admin GetById(int id)
        {
            return _context.Admins.FirstOrDefault(a => a.Id == id);
        }
    }
}
