using inOfficeApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.Repository.Interface
{
    public interface IEmployeeRepository
    {
        Employee Create(Employee employee);
        Employee GetByEmail(string email);
        Employee GetById(int id);
    }
}
