using inOffice.Repository.Interface;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public EmployeeController(JwtService jwtService, IEmployeeRepository employeeRepository)
        {
      
            _jwtService = jwtService;

        }

        [HttpGet("employee/home")]
        public IActionResult Employee()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var employee = _jwtService.EmployeeRoleVerification(authHeader);

                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [HttpGet("employee/informations")]
        public IActionResult Informations()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var employee = _jwtService.EmployeeRoleVerification(authHeader);

                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [HttpGet("employee/reservations")]
        public IActionResult Reservations()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var employee = _jwtService.EmployeeRoleVerification(authHeader);

                if (employee != null)
                {
                    return Ok(employee);
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        
    }
}
