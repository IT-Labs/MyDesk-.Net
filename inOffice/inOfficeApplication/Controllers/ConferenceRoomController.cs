using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    [Authorize]
    public class ConferenceRoomController : ControllerBase
    {
        private readonly IConferenceRoomService _conferenceRoomService;
        public ConferenceRoomController(IConferenceRoomService conferenceRoomService)
        {
            _conferenceRoomService = conferenceRoomService;
        }

        [HttpGet("employee/office-conferencerooms/{id}")]
        public IActionResult GetAllConferenceRoomsForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpGet("admin/office-conferencerooms/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllConferenceRooms(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpDelete("admin/office-conferencerooms/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(int id)
        {
            _conferenceRoomService.Delete(id);
            return Ok();
        }
    }
}
