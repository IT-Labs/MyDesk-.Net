namespace inOffice.BusinessLogicLayer.Requests
{
    public class ReservationRequest
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string EmployeeEmail { get; set; }
        public int DeskId { get; set; }
    }
}
