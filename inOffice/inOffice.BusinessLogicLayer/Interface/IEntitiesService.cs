using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEntitiesService
    {
        EntitiesResponse CreateNewDesks(EntitiesRequest request);

        ConferenceRoomsResponse ListAllConferenceRooms(int id);

        DesksResponse ListAllDesks(int id);

        EntitiesResponse UpdateDesks(UpdateRequest request);

        DeleteResponse DeleteEntity(DeleteRequest o);

        AllReviewsForEntity AllReviewsForEntity(int id);

    }
}
