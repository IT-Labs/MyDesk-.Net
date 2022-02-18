namespace inOfficeApplication.Models
{
    public class DeskMode
    {
        public int DeskId { get; set; }
        public Desk? Desk { get; set; }
        public int ModeId { get; set; }
        public Mode? Mode { get; set; }
        
    }
}
