using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
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
