namespace inOfficeApplication.Models
{
    public class Mode
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int OfficeId { get; set; }
        public Office? Office { get; set; }
      /*  public virtual ICollection<Desk>? Desks { get; set; }
        public virtual ICollection<ConferenceRoom>? ConferenceRooms { get; set; }*/
    }
}
