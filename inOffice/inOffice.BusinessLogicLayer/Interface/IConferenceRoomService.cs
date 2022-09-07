using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IConferenceRoomService
    {
        List<ConferenceRoomDto> GetOfficeConferenceRooms(int id, int? take = null, int? skip = null);
        void Delete(int id);
    }
}
