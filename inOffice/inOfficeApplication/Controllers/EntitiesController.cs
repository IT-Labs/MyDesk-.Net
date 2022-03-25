using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class EntitiesController : Controller
    {
        private readonly IEntitiesService _entitiesService;
        private readonly JwtService _jwtService;


        public EntitiesController(IEntitiesService entitiesService, JwtService jwtservice)
        {
            _entitiesService = entitiesService;
            _jwtService = jwtservice;
        }
        [HttpPost("admin/office-entities/{id}")]
        public ActionResult<EntitiesResponse> GenerateEntities(int id, EntitiesRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var admin = _jwtService.AdminRoleVerification(authHeader);

            if (admin != null)
            {

                dto.Id = id;

                var response = _entitiesService.CreateNewEntities(dto);

                if (response.Success == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            else return Unauthorized();
        }
        [HttpGet("admin/office-desks/{id}")]
        public ActionResult<IEnumerable<Desk>> GetAllDesks(int id)
        {

            string authHeader = Request.Headers[HeaderNames.Authorization];
            var admin = _jwtService.AdminRoleVerification(authHeader);
            if (admin != null)
            {
                var deskList = _entitiesService.ListAllDesks(id);
                if (deskList.sucess == true)
                {
                    return Ok(deskList.DeskList);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPut("admin/office-entities")]
        public ActionResult<EntitiesResponse> UpdateEntities(UpdateRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var admin = _jwtService.AdminRoleVerification(authHeader);

            if (admin != null){

                UpdateRequest request = new UpdateRequest();

                request.CheckedDesks = dto.CheckedDesks;
                request.UncheckedDesks = dto.UncheckedDesks;
                request.ConferenceRoomCapacity = dto.ConferenceRoomCapacity;

                var updateConfirmed = _entitiesService.UpdateEntities(request);

                if(updateConfirmed.Success == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Unauthorized();
            }
        }
        [HttpGet("admin/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoom>> GetAllConferenceRooms(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var admin = _jwtService.AdminRoleVerification(authHeader);
            if (admin != null) {

                var conferenceRoomList = _entitiesService.ListAllConferenceRooms(id);
                if(conferenceRoomList.Sucess == true)
                {
                    return Ok(conferenceRoomList.ConferenceRoomsList);
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Unauthorized();
            }
          
        }
    }
}
