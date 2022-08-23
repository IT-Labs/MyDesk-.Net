namespace inOffice.BusinessLogicLayer.Requests
{
    public class UpdateRequest
    {
        public List<DeskToUpdate> ListOfDesksToUpdate { get; set; }
    }

    public class DeskToUpdate
    {
        public int DeskId { get; set; }
        public bool Unavailable { get; set; }
        public bool SingleMonitor { get; set; }
        public bool DualMonitor { get; set; }
        public bool NearWindow { get; set; }    
    }
}
