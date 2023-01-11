using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyDesk.Data.DTO;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Utils;

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
            if (id != EmployeeDto.Id && EmployeeDto.Id != null)
            {
                return BadRequest("Id doesn't match");
            }

            EmployeeDto.Id = id;
            _employeeService.UpdateEmployee(EmployeeDto);

            return Ok();
        }
    }
}
