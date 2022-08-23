using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Requests
{
    public class ReservationRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ConferenceRoomDto? ConferenceRoom { get; set; }
        public DeskDto? Desk { get; set; } 
    }
}
