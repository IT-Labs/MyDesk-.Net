using FluentValidation.Results;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Requests;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class DeskController : ControllerBase
    {
        private readonly IDeskService _deskService;
        public DeskController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        [HttpGet("employee/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<DeskDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllDesksForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _deskService.GetOfficeDesks(id, take: take, skip: skip);

            return Ok(desks);
        }

        [HttpGet("admin/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<DeskDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllDesks(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _deskService.GetOfficeDesks(id, take: take, skip: skip);

            return Ok(desks);
        }

        [HttpPost("admin/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Create(int id, [FromBody] EntitiesRequest entitiesRequest)
        {
            EntitiesRequestValidation validationRules = new EntitiesRequestValidation();
            ValidationResult validationResult = validationRules.Validate(entitiesRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _deskService.Create(id, entitiesRequest.NumberOfDesks);
            return Ok();
        }

        [HttpPut("admin/office-desks")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Update([FromBody] List<DeskDto> desks)
        {
            DeskDtosValidation validationRules = new DeskDtosValidation();
            ValidationResult validationResult = validationRules.Validate(desks);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _deskService.Update(desks);
            return Ok();
        }

        [HttpDelete("admin/office-desks/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(int id)
        {
            _deskService.Delete(id);
            return Ok();
        }
    }
}
