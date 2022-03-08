namespace inOfficeApplication.Data.Models
{
    public class Employee : User 
    {
        public string? JobTitle { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
    }
}
