using MyDesk.Data.DTO;
using MyDesk.Data.Requests;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IReservationService
    {
        PaginationDto<ReservationDto> FutureReservations(string employeeEmail, int? take = null, int? skip = null);
        PaginationDto<ReservationDto> PastReservations(string employeeEmail, int? take = null, int? skip = null);
        void CoworkerReserve(ReservationRequest reservationRequest);
        void CancelReservation(int id);
    }
}
