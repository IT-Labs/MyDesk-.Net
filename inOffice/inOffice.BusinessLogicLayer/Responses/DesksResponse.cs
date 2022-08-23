using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class DesksResponse
    {
        public List<DeskDto> DeskList { get; set; }
        public bool sucess;
    }
}
