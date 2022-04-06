using inOfficeApplication.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class ConferenceRoom : BaseEntity
    {
        [Required]
        public int Capacity { get; set; }
        public int? ReservationId { get; set; }
        public virtual Reservation? Reservation { get; set; }

        public int? IndexForOffice { get; set; }    
        public int OfficeId { get; set; }
        public virtual Office? Office { get; set; }
        public virtual ICollection<ConferenceRoomMode>? ConferenceRoomModes { get; set; }

    }
}
