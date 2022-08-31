namespace inOfficeApplication.Data.DTO
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string? Reviews { get; set; }
        public string? ReviewOutput { get; set; }
        public ReservationDto Reservation { get; set; }
    }
}
