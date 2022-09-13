#nullable disable

namespace inOfficeApplication.Data.Entities
{
    public partial class Reservation
    {
        public Reservation()
        {
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int EmployeeId { get; set; }
        public int? ConferenceRoomId { get; set; }
        public int? DeskId { get; set; }

        public virtual ConferenceRoom ConferenceRoom { get; set; }
        public virtual Desk Desk { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}