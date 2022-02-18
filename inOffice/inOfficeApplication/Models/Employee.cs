namespace inOfficeApplication.Models
{
    public class Employee : User
    {
        public int Id { get; set; }
        public string? JobTitle { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
