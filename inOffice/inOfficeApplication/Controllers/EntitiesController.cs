using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class EntitiesController : Controller
    {
        private readonly IEntitiesService _entitiesService;

        public EntitiesController(IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        [HttpGet("entity/reviews/{id}")]
        public ActionResult<AllReviewsForEntity> AllEntitiesForDesk(int id)
        {
            AllReviewsForEntity response = _entitiesService.AllReviewsForEntity(id);
            if (response.Success == true)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("admin/office-entities/{id}")]
        public ActionResult<EntitiesResponse> GenerateEntities(int id, EntitiesRequest dto)
        {
            if (dto.NumberOfDesks < 1 || dto.NumberOfDesks > 500)
            {
                return BadRequest("Maximum number of desks to be created is 500");
            }

            EntitiesResponse response = _entitiesService.CreateNewDesks(id, dto.NumberOfDesks);

            if (response.Success == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest(response);
            }
        }

        [HttpGet("admin/office-desks/{id}")]
        public ActionResult<DesksResponse> GetAllDesks(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            DesksResponse deskList = _entitiesService.ListAllDesks(id, take: take, skip: skip);
            if (deskList.sucess == true)
            {
                return Ok(deskList);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("admin/entity")]
        public ActionResult<DeleteResponse> DeleteEntity(DeleteRequest dto)
        {
            DeleteResponse deleteResponse = _entitiesService.DeleteEntity(dto);

            if (deleteResponse.Success == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("admin/office-entities")]
        public ActionResult<EntitiesResponse> UpdateEntities(UpdateRequest dto)
        {
            EntitiesResponse entitiesResponse = _entitiesService.UpdateDesks(dto);

            if (entitiesResponse.Success == true)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("admin/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoomDto>> GetAllConferenceRooms(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            ConferenceRoomsResponse conferenceRoomList = _entitiesService.ListAllConferenceRooms(id, take: take, skip: skip);
            if (conferenceRoomList.Sucess == true)
            {
                return Ok(conferenceRoomList.ConferenceRoomsList);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("employee/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoomDto>> GetAllConferenceRoomsForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            ConferenceRoomsResponse conferenceRoomList = _entitiesService.ListAllConferenceRooms(id, take: take, skip: skip);
            if (conferenceRoomList.Sucess == true)
            {
                return Ok(conferenceRoomList.ConferenceRoomsList);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("employee/office-desks/{id}")]
        public ActionResult<IEnumerable<DeskDto>> GetAllDesksForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            DesksResponse deskList = _entitiesService.ListAllDesks(id, take: take, skip: skip);
            if (deskList.sucess == true)
            {
                return Ok(deskList.DeskList);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
