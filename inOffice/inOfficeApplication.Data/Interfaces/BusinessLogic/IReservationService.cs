using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Requests;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IReservationService
    {
        List<ReservationDto> FutureReservations(string employeeEmail, int? take = null, int? skip = null);
        void CancelReservation(int id);
        List<ReservationDto> PastReservations(string employeeEmail, int? take = null, int? skip = null);
        PaginationDto<ReservationDto> AllReservations(int? take = null, int? skip = null);
        void CoworkerReserve(ReservationRequest reservationRequest);
    }
}
