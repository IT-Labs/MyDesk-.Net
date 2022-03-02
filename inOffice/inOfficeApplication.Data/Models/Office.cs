using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Office
    {
        public int Id { get; set; }
        [Required]
        //[MaxLength(55)]
        public string? Name { get; set; }
        [Required]
        //[MaxLength(50)]
     
        
        //  public string? CurrentMode { get; set; }
        public string? OfficeImage { get; set; }

        public ICollection<Desk>? Desks { get; set; }
        public ICollection<Mode>? Modes { get; set; }
        public ICollection<ConferenceRoom>? ConferenceRooms { get; set; }
    }
}
