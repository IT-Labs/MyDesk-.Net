namespace inOfficeApplication.Data.Models
{
    public class Employee : User 
    {
        public string? JobTitle { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
