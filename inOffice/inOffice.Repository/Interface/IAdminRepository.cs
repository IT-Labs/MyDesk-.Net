using inOfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.Repository.Interface
{
    public interface IAdminRepository
    {
        Admin Create(Admin admin);
        Admin GetByEmail(string email); 
        Admin GetById(int id); 


    }
}
