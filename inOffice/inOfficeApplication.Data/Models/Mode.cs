using inOfficeApplication.Data.Models;

namespace inOfficeApplication.Data.Models
{
    public class Mode
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int OfficeId { get; set; }
        public Office? Office { get; set; }
        public virtual ICollection<DeskMode>? DeskModes { get; set; }
        public virtual ICollection<ConferenceRoomMode>? ConferenceRoomModes { get; set; }
    }
}
