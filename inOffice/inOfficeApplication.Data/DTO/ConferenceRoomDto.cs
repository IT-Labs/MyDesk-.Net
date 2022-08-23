namespace inOfficeApplication.Data.DTO
{
    public class ConferenceRoomDto
    {
        public int? Id { get; set; }
        public int? Capacity { get; set; }
        public int? IndexForOffice { get; set; }
        public int? OfficeId { get; set; }
        public ReservationDto? Reservation { get; set; }
    }
}
