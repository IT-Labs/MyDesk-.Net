using MyDesk.Data.Entities;

namespace MyDesk.Data.Interfaces.Repository
{
    public interface IConferenceRoomRepository
    {
        ConferenceRoom Get(int id, bool? includeReservations = null, bool? includeReviews = null);
        List<ConferenceRoom> GetOfficeConferenceRooms(int officeId, bool? includeReservations = null, int? take = null, int? skip = null);
        void SoftDelete(ConferenceRoom conferenceRoom);
    }
}
