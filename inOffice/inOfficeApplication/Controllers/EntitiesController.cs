using FluentValidation.Results;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class EntitiesController : Controller
    {
        private readonly IEntitiesService _entitiesService;

        public EntitiesController(IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        [HttpGet("entity/reviews/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ReviewDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AllEntitiesForDesk(int id)
        {
            List<ReviewDto> reviews = _entitiesService.AllReviewsForEntity(id);
            return Ok(reviews);
        }

        [HttpPost("admin/office-entities/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GenerateEntities(int id, [FromBody] EntitiesRequest entitiesRequest)
        {
            EntitiesRequestValidation validationRules = new EntitiesRequestValidation();
            ValidationResult validationResult = validationRules.Validate(entitiesRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _entitiesService.CreateNewDesks(id, entitiesRequest.NumberOfDesks);
            return Ok();
        }

        [HttpGet("admin/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<DeskDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllDesks(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _entitiesService.ListAllDesks(id, take: take, skip: skip);

            return Ok(desks);
        }

        [HttpDelete("admin/entity")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult DeleteEntity([FromBody] DeleteRequest deleteRequest)
        {
            DeleteRequestValidation validationRules = new DeleteRequestValidation();
            ValidationResult validationResult = validationRules.Validate(deleteRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _entitiesService.DeleteEntity(deleteRequest);
            return Ok();
        }

        [HttpPut("admin/office-entities")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult UpdateEntities([FromBody] List<DeskDto> desks)
        {
            DeskDtosValidation validationRules = new DeskDtosValidation();
            ValidationResult validationResult = validationRules.Validate(desks);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _entitiesService.UpdateDesks(desks);
            return Ok();
        }

        [HttpGet("admin/office-conferencerooms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ConferenceRoomDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllConferenceRooms(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _entitiesService.ListAllConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpGet("employee/office-conferencerooms/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ConferenceRoomDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllConferenceRoomsForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ConferenceRoomDto> conferenceRooms = _entitiesService.ListAllConferenceRooms(id, take: take, skip: skip);

            return Ok(conferenceRooms);
        }

        [HttpGet("employee/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<DeskDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllDesksForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _entitiesService.ListAllDesks(id, take: take, skip: skip);

            return Ok(desks);
        }
    }
}
