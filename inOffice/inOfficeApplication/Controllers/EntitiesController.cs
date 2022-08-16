﻿using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpGet("entity/reviews/{id}")]
        public ActionResult<AllReviewsForEntity> AllEntitiesForDesk(int id)
        {
            try
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
            catch (Exception _)
            {
                return BadRequest();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpPost("admin/office-entities/{id}")]
        public ActionResult<EntitiesResponse> GenerateEntities(int id, EntitiesRequest dto)
        {
            try
            {
                dto.Id = id;

                EntitiesResponse response = _entitiesService.CreateNewDesks(dto);

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpGet("admin/office-desks/{id}")]
        public ActionResult<DesksResponse> GetAllDesks(int id)
        {
            try
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpDelete("admin/entity")]
        public ActionResult<DeleteResponse> DeleteEntity(DeleteRequest dto)
        {
            try
            {
                DeleteResponse deleteResponse = _entitiesService.DeleteEntity(dto);

                if (deleteResponse.Success == true)
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpPut("admin/office-entities")]
        public ActionResult<EntitiesResponse> UpdateEntities(UpdateRequest dto)
        {
            try
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpGet("admin/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoom>> GetAllConferenceRooms(int id)
        {
            try
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.EmployeeRole)]
        [HttpGet("employee/office-conferencerooms/{id}")]
        public ActionResult<IEnumerable<ConferenceRoom>> GetAllConferenceRoomsForEmployee(int id)
        {
            try
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.EmployeeRole)]
        [HttpGet("employee/office-desks/{id}")]
        public ActionResult<IEnumerable<Desk>> GetAllDesksForEmployee(int id)
        {
            try
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
    }
}
