using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IReservationService
    {
       ReservationResponse Reserve(ReservationRequest o, Employee employee);
       EmployeeReservationsResponse EmployeeReservations(Employee employee);
       CancelReservationResponse CancelReservation(int id);
       EmployeeReservationsResponse PastReservations(Employee employee);
       CreateReviewResponse CreateReview(CreateReviewRequest createReviewRequest);
       ReviewResponse ShowReview(int id);

    }
}
