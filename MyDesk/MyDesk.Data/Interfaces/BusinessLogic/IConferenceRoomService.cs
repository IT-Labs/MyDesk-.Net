using MyDesk.Data.DTO;

namespace MyDesk.Data.Interfaces.BusinessLogic
{
    public interface IConferenceRoomService
    {
        List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null);
        void Delete(int id);
    }
}
