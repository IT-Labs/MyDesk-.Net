using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Requests;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IReservationService
    {
        PaginationDto<ReservationDto> FutureReservations(string employeeEmail, int? take = null, int? skip = null);
        PaginationDto<ReservationDto> PastReservations(string employeeEmail, int? take = null, int? skip = null);
        void CoworkerReserve(ReservationRequest reservationRequest);
        void CancelReservation(int id);
    }
}
