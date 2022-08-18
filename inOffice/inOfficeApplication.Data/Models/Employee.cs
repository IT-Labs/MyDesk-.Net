using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Employee : BaseEntity 
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
        public string? JobTitle { get; set; }
        public virtual ICollection<Reservation>? Reservations { get; set; }
    }
}
