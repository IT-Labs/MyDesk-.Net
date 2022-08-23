using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface IReservationRepository
    {
        Reservation Get(int id, bool? includeDesk = null, bool? includeOffice = null, bool? includeonferenceRoom = null, bool? includeReviews = null);
        List<Reservation> GetAll(bool? includeEmployee = null, bool? includeDesk = null, bool? includeOffice = null, int? take = null, int? skip = null);
        List<Reservation> GetEmployeeReservations(int employeeId, bool? includeDesk = null, bool? includeOffice = null);
        List<Reservation> GetDeskReservations(int deskId, bool? includeReview = null, bool? includeEmployee = null);
        void Insert(Reservation reservation);
        void SoftDelete(Reservation reservation);
    }
}
