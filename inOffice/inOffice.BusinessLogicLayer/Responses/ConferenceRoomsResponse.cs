using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Responses
{
    public class ConferenceRoomsResponse
    {
       public List<ConferenceRoomDto> ConferenceRoomsList { get; set; }
       public bool Sucess { get; set; }
    }
}
