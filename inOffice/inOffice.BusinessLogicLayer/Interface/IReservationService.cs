using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Entities;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReservationService
    {
       EmployeeReservationsResponse EmployeeReservations(Employee employee);
       CancelReservationResponse CancelReservation(int id);
       EmployeeReservationsResponse PastReservations(Employee employee);
       AllReservationsResponse AllReservations(int? take = null, int? skip = null);
       ReservationResponse CoworkerReserve(CoworkerReservationRequest dto);
    }
}
