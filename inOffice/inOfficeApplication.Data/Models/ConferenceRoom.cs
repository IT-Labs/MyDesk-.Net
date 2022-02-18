using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Models
{
    public class ConferenceRoom
    {
        public int Id { get; set; }
        [Required]
        
        public int Capacity { get; set; }
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        public int OfficeId { get; set; }
        public Office? Office { get; set; }
       // public virtual ICollection<Mode>? Modes { get; set; }

    }
}
