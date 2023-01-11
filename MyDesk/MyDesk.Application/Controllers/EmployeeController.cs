using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDesk.Application.Validations;
using MyDesk.Data.DTO;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Utils;
using FluentValidation.Results;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("employee/all")]
        public IActionResult AllEmployees()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<EmployeeDto> result = _employeeService.GetAll(take: take, skip: skip);

            return Ok(result);
        }
        [HttpPut("admin/employee/{id}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateEmployee(int id, EmployeeDto EmployeeDto)
        {
            var validationRules = new EmployeeDtoValidation();
            ValidationResult validationResult = validationRules.Validate(EmployeeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            EmployeeDto.Id = id;
            _employeeService.UpdateEmployee(EmployeeDto);

            return Ok();
        }
    }
}
