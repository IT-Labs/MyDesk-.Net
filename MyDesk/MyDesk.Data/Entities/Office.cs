#nullable disable

namespace MyDesk.Data.Entities
{
    public partial class Office
    {
        public Office()
        {
            ConferenceRooms = new HashSet<ConferenceRoom>();
            Desks = new HashSet<Desk>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string OfficeImage { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<ConferenceRoom> ConferenceRooms { get; set; }
        public virtual ICollection<Desk> Desks { get; set; }
    }
}