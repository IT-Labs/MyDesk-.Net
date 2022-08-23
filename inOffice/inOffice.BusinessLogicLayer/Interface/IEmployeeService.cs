using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        Employee GetByEmail(string email);
        Employee GetByEmailAndPassword(string email, string password);
        List<CustomEmployee> GetAll();
        void Create(Employee employee);
    }
}
