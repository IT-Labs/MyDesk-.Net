using inOfficeApplication.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace inOfficeApplication.Data.Models
{
    public class Desk : BaseEntity
    {
        [Required]
        public string? Categories { get; set; }
        public int? ReservationId { get; set; }
        public int? IndexForOffice { get; set; }
        public int? CategorieId { get; set; }
        public virtual Reservation? Reservation { get; set; }
        public int OfficeId { get; set; }
        public virtual Office? Office { get; set; }
        public virtual ICollection<DeskMode>? DeskModes { get; set; }

        public virtual ICollection<DeskCategories> DeskCategories { get; set; }
    }
}
