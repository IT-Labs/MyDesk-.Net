using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReservationService
    {
       ReservationResponse Reserve(ReservationRequest request, Employee employee);
       EmployeeReservationsResponse EmployeeReservations(Employee employee);
       CancelReservationResponse CancelReservation(int id);
       EmployeeReservationsResponse PastReservations(Employee employee);
       AllReservationsResponse AllReservations();
       ReservationResponse CoworkerReserve(CoworkerReservationRequest dto); 

    }
}
