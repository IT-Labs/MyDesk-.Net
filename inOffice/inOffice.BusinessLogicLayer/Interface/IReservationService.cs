using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReservationService
    {
        List<ReservationDto> EmployeeReservations(string employeeEmail);
        void CancelReservation(int id);
        List<ReservationDto> PastReservations(string employeeEmail);
        PaginationDto<ReservationDto> AllReservations(int? take = null, int? skip = null);
        void CoworkerReserve(ReservationRequest reservationRequest);
    }
}
