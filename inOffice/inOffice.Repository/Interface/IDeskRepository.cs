using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IDeskRepository
    {
        Desk Get(int id);
        List<Desk> GetOfficeDesks(int officeId, int? take = null, int? skip = null);
        void BulkInsert(List<Desk> desks);
        void Update(Desk desk);
        void SoftDelete(Desk desk);
    }
}
