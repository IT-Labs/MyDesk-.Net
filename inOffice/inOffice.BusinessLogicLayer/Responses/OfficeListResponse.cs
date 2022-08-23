using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class OfficeListResponse
    {
        public List<OfficeDto> Offices { get; set; }
        public bool Success { get; set; }
    }
}
