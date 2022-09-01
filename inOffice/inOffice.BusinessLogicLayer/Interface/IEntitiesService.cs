using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.DTO;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEntitiesService
    {
        void CreateNewDesks(int officeId, int numberOfInstancesToCreate);
        List<ConferenceRoomDto> ListAllConferenceRooms(int id, int? take = null, int? skip = null);
        List<DeskDto> ListAllDesks(int id, int? take = null, int? skip = null);
        void UpdateDesks(List<DeskDto> desks);
        void DeleteEntity(DeleteRequest deleteRequest);
        List<ReviewDto> AllReviewsForEntity(int id);
    }
}
