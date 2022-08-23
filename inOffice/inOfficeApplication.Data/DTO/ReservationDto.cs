namespace inOfficeApplication.Data.DTO
{
    public class ReservationDto
    {
        public int Id { get; set; }
        public int? DeskId { get; set; }
        public int? ConfId { get; set; }
        public int? ConferenceRoomId { get; set; }
        public int? ReviewId { get; set; }
        public int? EmployeeId { get; set; }
        public int? ConfRoomIndex { get; set; }
        public int? DeskIndex { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? OfficeName { get; set; }
        public int? IndexForOffice { get; set; }
        public EmployeeDto? Employee { get; set; }
        public DeskDto? Desk { get; set; }
        public ReviewDto? Review { get; set; }
    }
}
