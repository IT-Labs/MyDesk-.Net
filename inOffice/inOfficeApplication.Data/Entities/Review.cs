#nullable disable

namespace inOfficeApplication.Data.Entities
{
    public partial class Review
    {
        public int Id { get; set; }
        public string Reviews { get; set; }
        public string ReviewOutput { get; set; }
        public bool? IsDeleted { get; set; }
        public int ReservationId { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}