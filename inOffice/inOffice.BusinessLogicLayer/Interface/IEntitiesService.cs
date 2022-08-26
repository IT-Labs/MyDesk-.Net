using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEntitiesService
    {
        EntitiesResponse CreateNewDesks(int officeId, int numberOfInstancesToCreate);

        ConferenceRoomsResponse ListAllConferenceRooms(int id, int? take = null, int? skip = null);

        DesksResponse ListAllDesks(int id, int? take = null, int? skip = null);

        EntitiesResponse UpdateDesks(UpdateRequest request);

        DeleteResponse DeleteEntity(DeleteRequest o);

        AllReviewsForEntity AllReviewsForEntity(int id);

    }
}
