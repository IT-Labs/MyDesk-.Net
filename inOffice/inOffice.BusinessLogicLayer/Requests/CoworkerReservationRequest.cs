namespace inOffice.BusinessLogicLayer.Requests
{
    public class CoworkerReservationRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string CoworkerMail { get; set; }
        public int DeskId { get; set; }
    }
}
