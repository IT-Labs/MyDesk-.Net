namespace MyDesk.Data.DTO
{
    public class ConferenceRoomDto
    {
        public int? Id { get; set; }
        public int? Capacity { get; set; }
        public int? IndexForOffice { get; set; }
        public OfficeDto Office { get; set; }
        public List<ReservationDto>? Reservations { get; set; }
    }
}
