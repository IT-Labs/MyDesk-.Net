using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inOffice.BusinessLogicLayer.Interface
{
    public interface IEntitiesService
    {
        EntitiesResponse CreateNewEntities(EntitiesRequest o);

        ConferenceRoomsResponse ListAllConferenceRooms(int id);

        DesksResponse ListAllDesks(int id);

        EntitiesResponse UpdateEntities(UpdateRequest o);

        DeleteResponse DeleteEntity(DeleteRequest o);

        AllReviewsForEntity AllReviewsForEntity(int id);

    }
}
