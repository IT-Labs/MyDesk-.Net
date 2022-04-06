using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace inOfficeApplication.Data.Models
{
    public class Reservation : BaseEntity
    {
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public int EmployeeId { get; set;}
        public virtual Employee? Employee { get; set; }
        public int? DeskId { get; set; }
        public virtual Desk? Desk { get; set; }
        public int? ConferenceRoomId { get; set; }
        public virtual ConferenceRoom? ConferenceRoom { get; set; }
        public int? ReviewId { get; set; }
        public virtual Review? Review { get; set; }
    }
}
