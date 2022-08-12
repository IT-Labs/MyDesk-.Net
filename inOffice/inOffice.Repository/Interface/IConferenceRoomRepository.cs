using inOfficeApplication.Data.Models;

namespace inOffice.Repository.Interface
{
    public interface IConferenceRoomRepository
    {
        ConferenceRoom Get(int id, bool? includeReservation = null);
        List<ConferenceRoom> GetOfficeConferenceRooms(int officeId);
        void Update(ConferenceRoom conferenceRoom);
        void BulkUpdate(List<ConferenceRoom> conferenceRooms);
        void Delete(ConferenceRoom conferenceRoom);
    }
}
