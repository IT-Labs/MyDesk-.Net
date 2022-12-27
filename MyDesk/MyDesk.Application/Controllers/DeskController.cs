using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FluentValidation.Results;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.DTO;
using MyDesk.Data.Requests;
using MyDesk.Data.Utils;
using MyDesk.Application.Validations;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    [Authorize]
    public class DeskController : ControllerBase
    {
        private readonly IDeskService _deskService;
        public DeskController(IDeskService deskService)
        {
            _deskService = deskService;
        }

        [HttpGet("employee/office-desks/{id}")]
        public IActionResult GetAllDesksForEmployee(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _deskService.GetOfficeDesks(id, take: take, skip: skip);

            return Ok(desks);
        }

        [HttpGet("admin/office-desks/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllDesks(int id)
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<DeskDto> desks = _deskService.GetOfficeDesks(id, take: take, skip: skip);

            return Ok(desks);
        }

        [HttpPost("admin/office-desks/{id}")]
        [Authorize(Roles = "ADMIN")]
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
        [Authorize(Roles = "ADMIN")]
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
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(int id)
        {
            _deskService.Delete(id);
            return Ok();
        }
    }
}
