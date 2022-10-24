using inOfficeApplication.Data.Entities;

namespace inOfficeApplication.Data.Interfaces.Repository
{
    public interface IReservationRepository
    {
        Reservation Get(int id, bool? includeDesk = null, bool? includeOffice = null, bool? includeonferenceRoom = null, bool? includeReviews = null);
        List<Reservation> GetEmployeeReservations(int employeeId, bool? includeDesk = null, bool? includeConferenceRoom = null);
        Tuple<int?, List<Reservation>> GetFutureReservations(int? employeeId = null, bool? includeDesk = null, bool? includeConferenceRoom = null, bool? includeOffice = null, int? take = null, int? skip = null);
        Tuple<int?, List<Reservation>> GetPastReservations(int? employeeId = null, bool? includeDesk = null, bool? includeConferenceRoom = null, bool? includeOffice = null, bool? includeReviews = null, int? take = null, int? skip = null);
        List<Reservation> GetDeskReservations(int deskId, bool? includeEmployee = null);
        List<Reservation> GetPastDeskReservations(int deskId, bool? includeReview = null);
        void Insert(Reservation reservation);
        void SoftDelete(Reservation reservation);
    }
}
