namespace inOfficeApplication.Models
{
    public class ConferenceRoomMode
    {
        public int ConferenceRoomId { get; set; }
        public ConferenceRoom? ConferenceRoom { get; set; }
        public int ModeId { get; set; }
        public Mode? Mode { get; set; }
    }
}
