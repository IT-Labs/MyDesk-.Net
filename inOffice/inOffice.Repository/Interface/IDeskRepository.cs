using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface IDeskRepository
    {
        Desk Get(int id, bool? includeReservations = null, bool? includeReviews = null);
        int GetHighestDeskIndexForOffice(int officeId);
        List<Desk> GetOfficeDesks(int officeId, bool? includeCategory = null, bool? includeReservations = null, bool? includeEmployees = null, int? take = null, int? skip = null);
        void BulkInsert(List<Desk> desks);
        void Update(Desk desk);
        void SoftDelete(Desk desk);
    }
}
