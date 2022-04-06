using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Office : BaseEntity
    {
        
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? OfficeImage { get; set; }
        public virtual ICollection<Desk>? Desks { get; set; }
        public virtual ICollection<Mode>? Modes { get; set; }
        public virtual ICollection<ConferenceRoom>? ConferenceRooms { get; set; }
    }
}
