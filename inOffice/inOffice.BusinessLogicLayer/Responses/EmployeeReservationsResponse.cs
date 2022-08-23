using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class EmployeeReservationsResponse
    {
        public List<ReservationDto> CustomReservationResponses { get; set; }
        public bool Success { get; set; }

        public EmployeeReservationsResponse()
        {
            CustomReservationResponses = new List<ReservationDto>();
        }
    }
}
