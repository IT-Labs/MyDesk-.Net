using inOfficeApplication.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Models
{
    public class Desk
    {
        public int Id { get; set; }
        // public List<string>? Categories { get; set; }
        [Required]
        public string? Categories { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
        public int OfficeId { get; set; }
        public Office? Office { get; set; }
        public virtual ICollection<DeskMode>? DeskModes { get; set; }
    }
}
