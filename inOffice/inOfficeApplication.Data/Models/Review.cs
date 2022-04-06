using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Review : BaseEntity
    {
        [MaxLength(200)]
        public string? Reviews { get; set; }
        public string? ReviewOutput { get; set; }
        public int ReservationId { get; set; }
        public virtual Reservation? Reservation { get; set; }
    }
}
