using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class AllReservationsResponse
    {
        public List<ReservationDto>? Reservations { get; set; }
        public int TotalReservations { get; set; }
        public bool Success { get; set; }
    }
}
