using MyDesk.Core.DTO;
using MyDesk.Core.Requests;

namespace MyDesk.Core.Interfaces.BusinessLogic
{
    public interface IReservationService
    {
        PaginationDto<ReservationDto> FutureReservations(string employeeEmail, int? take = null, int? skip = null);
        PaginationDto<ReservationDto> PastReservations(string employeeEmail, int? take = null, int? skip = null);
        void CoworkerReserve(ReservationRequest reservationRequest);
        void CancelReservation(int id);
    }
}
