using MyDesk.Core.DTO;

namespace MyDesk.Core.DTO
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EmployeeDto? Employee { get; set; }
        public DeskDto? Desk { get; set; }
        public ConferenceRoomDto? ConferenceRoom { get; set; }
        public List<ReviewDto>? Reviews { get; set; }
    }
}
