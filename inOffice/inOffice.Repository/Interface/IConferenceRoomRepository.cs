using inOfficeApplication.Data.Entities;

namespace inOffice.Repository.Interface
{
    public interface IConferenceRoomRepository
    {
        ConferenceRoom Get(int id, bool? includeReservations = null, bool? includeReviews = null);
        List<ConferenceRoom> GetOfficeConferenceRooms(int officeId, bool? includeReservations = null, int? take = null, int? skip = null);
        void SoftDelete(ConferenceRoom conferenceRoom);
    }
}
