using MyDesk.Core.DTO;

namespace MyDesk.Core.Interfaces.BusinessLogic
{
    public interface IConferenceRoomService
    {
        List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null);
        void Delete(int id);
    }
}
