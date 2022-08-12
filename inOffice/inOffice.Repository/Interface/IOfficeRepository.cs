using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IOfficeRepository
    {
        Office Get(int id);
        Office GetByName(string name);
        List<Office> GetAll();
        void Insert(Office office);
        void Update(Office office);
        void SoftDelete(Office office);
    }
}
