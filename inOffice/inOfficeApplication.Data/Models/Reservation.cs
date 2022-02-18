using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public int EmployeeId { get; set;}
        public Employee? Employee { get; set; }

        public Desk? Desk { get; set; }
        public ConferenceRoom? ConferenceRoom { get; set; }
        public Review? Review { get; set; }
    }
}
