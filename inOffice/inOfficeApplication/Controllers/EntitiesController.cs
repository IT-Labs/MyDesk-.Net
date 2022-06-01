using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        //TODO: implement review for desk
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet("entity/reviews/{id}")]
        public ActionResult<AllReviewsForEntity> AllEntitiesForDesk(int id)
        {
            try {
                var response = _entitiesService.AllReviewsForEntity(id);
                if (response.Success == true)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception _) {
                return BadRequest();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPost("admin/office-entities/{id}")]
        public ActionResult<EntitiesResponse> GenerateEntities(int id, EntitiesRequest dto)
        {
           try
            {

                dto.Id = id;

                var response = _entitiesService.CreateNewEntities(dto);

                if (response.Success == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(response);
                }
               
            }
            catch (Exception _)
            {
                return BadRequest();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet("admin/office-desks/{id}")]
        public ActionResult<DesksResponse> GetAllDesks(int id)
        {

            try
            {
                var deskList = _entitiesService.ListAllDesks(id);
                if (deskList.sucess == true)
                {
                    return Ok(deskList);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpDelete("admin/entity")]
        public ActionResult<DeleteResponse> DeleteEntity(DeleteRequest dto)
        {
           try
            {
                var deleteResponse = _entitiesService.DeleteEntity(dto);

                if(deleteResponse.Success == true)
                {
                    return Ok();
                }
                else
                {
                  return NotFound();
                }
            }
            catch (Exception _)    
            {
                return Unauthorized();
            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPut("admin/office-entities")]
        public ActionResult<EntitiesResponse> UpdateEntities(UpdateRequest dto)
        {
           try{

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
            catch(Exception _) 
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpGet("admin/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoom>> GetAllConferenceRooms(int id)
        {
            
            try {

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
            catch (Exception _)
            {
                return Unauthorized();
            }
          
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "EMPLOYEE")]
        [HttpGet("employee/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoom>> GetAllConferenceRoomsForEmployee(int id)
        {
            try
            {
                var conferenceRoomList = _entitiesService.ListAllConferenceRooms(id);
                if (conferenceRoomList.Sucess == true)
                {
                    return Ok(conferenceRoomList.ConferenceRoomsList);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception _)
            {
                return Unauthorized();

            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "EMPLOYEE")]
        [HttpGet("employee/office-desks/{id}")]
        public ActionResult<IEnumerable<Desk>> GetAllDesksForEmployee(int id)
        {

            try{
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
            catch(Exception _)
            {
                return Unauthorized();
            }

        }


    }
}
