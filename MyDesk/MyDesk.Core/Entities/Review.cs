#nullable disable

using MyDesk.Core.Database;

namespace MyDesk.Core.Entities
{
    public class Review : IEntity<int>
    {
        public int Id { get; set; }
        public string Reviews { get; set; }
        public string ReviewOutput { get; set; }
        public bool? IsDeleted { get; set; }
        public int ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}