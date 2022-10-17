using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class ConferenceRoomController : ControllerBase
    {
        private readonly IConferenceRoomService _conferenceRoomService;
        public ConferenceRoomController(IConferenceRoomService conferenceRoomService)
        {
            _conferenceRoomService = conferenceRoomService;
        }

        [HttpGet("employee/office-conferencerooms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ConferenceRoomDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllConferenceRoomsForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpGet("admin/office-conferencerooms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ConferenceRoomDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllConferenceRooms(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpDelete("admin/office-conferencerooms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(int id)
        {
            _conferenceRoomService.Delete(id);
            return Ok();
        }
    }
}
