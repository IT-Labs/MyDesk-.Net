using inOfficeApplication.Data.DTO;

namespace inOfficeApplication.Data.Interfaces.BusinessLogic
{
    public interface IConferenceRoomService
    {
        List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null);
        void Delete(int id);
    }
}
