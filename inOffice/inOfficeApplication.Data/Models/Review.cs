using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Models
{
    public class Review
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Reviews { get; set; }
        public string? ReviewOutput { get; set; }

        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }
    }
}
