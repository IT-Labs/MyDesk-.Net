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
        public IActionResult SetAsAdmin(int id)
        {
            _employeeService.SetEmployeeAsAdmin(id);
            return Ok();
        }
    }
}
