using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IReservationRepository
    {
        Reservation Get(int id, bool? includeDesk = null, bool? includeOffice = null, bool? includeonferenceRoom = null);
        List<Reservation> GetAll(bool? includeEmployee = null, bool? includeDesk = null, bool? includeOffice = null);
        List<Reservation> GetEmployeeReservations(int employeeId, bool? includeDesk = null, bool? includeOffice = null);
        void Insert(Reservation reservation);
        void Update(Reservation reservation);
        void Delete(Reservation reservation);
    }
}
