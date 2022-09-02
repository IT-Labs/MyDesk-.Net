using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("employee/all")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<EmployeeDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AllEmployees()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<EmployeeDto> result = _employeeService.GetAll(take: take, skip: skip);

            return Ok(result);
        }

        [HttpPut("admin/employee/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult SetAsAdmin(int id)
        {
            _employeeService.SetEmployeeAsAdmin(id);
            return Ok();
        }
    }
}
