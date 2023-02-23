using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using FluentValidation.Results;
using MyDesk.Core.DTO;
using MyDesk.Application.Validations;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.BusinessLogicLayer.Utils;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    [Authorize]

    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;
        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet("employee/offices")]
        public IActionResult GetAllOfficesForEmployee()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("admin/offices")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("employee/office/image/{id}")]
        public IActionResult ImageUrlForEmployee(int id)
        {
            OfficeDto office = _officeService.GetDetailsForOffice(id);
            return Ok(office.OfficeImage);
        }

        [HttpGet("admin/office/image/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult ImageUrl(int id)
        {
            OfficeDto officeDto = _officeService.GetDetailsForOffice(id);
            return Ok(officeDto.OfficeImage);
        }

        [HttpPost("admin/office")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddNewOffice([FromBody] OfficeDto officeDto)
        {
            OfficeDtoValidation validationRules = new OfficeDtoValidation();
            ValidationResult validationResult = validationRules.Validate(officeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _officeService.CreateNewOffice(officeDto);

            return Created("Success", officeDto);
        }

        [HttpPut("admin/office/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Edit(int id, [FromBody] OfficeDto officeDto)
        {
            OfficeDtoValidation validationRules = new OfficeDtoValidation();
            ValidationResult validationResult = validationRules.Validate(officeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            officeDto.Id = id;
            _officeService.UpdateOffice(officeDto);

            return Ok();
        }

        [HttpDelete("admin/office/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Delete(int id)
        {
            _officeService.DeleteOffice(id);
            return Ok();
        }
    }
}
