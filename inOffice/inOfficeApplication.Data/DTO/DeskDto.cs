namespace inOfficeApplication.Data.DTO
{
    public class DeskDto
    {
        public int? Id { get; set; }
        public int? IndexForOffice { get; set; }
        public List<ReservationDto>? Reservations { get; set; }
        public CategoryDto Categories { get; set; }
    }
}
