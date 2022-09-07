using FluentValidation.Results;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class OfficeController : ControllerBase
    {
        private readonly IOfficeService _officeService;
        private readonly OfficeDtoValidation _validationRules;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
            _validationRules = new OfficeDtoValidation();
        }

        [HttpGet("employee/offices")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<OfficeDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllOfficesForEmployee()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("admin/offices")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<OfficeDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("employee/office/image/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ImageUrlForEmployee(int id)
        {
            OfficeDto office = _officeService.GetDetailsForOffice(id);
            return Ok(office.OfficeImage);
        }

        [HttpGet("admin/office/image/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ImageUrl(int id)
        {
            OfficeDto officeDto = _officeService.GetDetailsForOffice(id);
            return Ok(officeDto.OfficeImage);
        }

        [HttpPost("admin/office")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AddNewOffice([FromBody] OfficeDto officeDto)
        {
            ValidationResult validationResult = _validationRules.Validate(officeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _officeService.CreateNewOffice(officeDto);

            return Created("Success", officeDto);
        }

        [HttpPut("admin/office/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Edit(int id, [FromBody] OfficeDto officeDto)
        {
            ValidationResult validationResult = _validationRules.Validate(officeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            officeDto.Id = id;
            _officeService.UpdateOffice(officeDto);

            return Ok();
        }

        [HttpDelete("admin/office/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Delete(int id)
        {
            _officeService.DeleteOffice(id);
            return Ok();
        }
    }
}
