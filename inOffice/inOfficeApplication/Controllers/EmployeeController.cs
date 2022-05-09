using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public EmployeeController(IOfficeService officeService)
        {
            _officeService = officeService; 
        }

       /* [HttpGet("employee/home")]
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
        }*/


       /* [HttpGet("employee/my-account/informations")]
        public IActionResult Informations()
        {
            try
            {
                

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
        }*/

       /* [HttpGet("employee/my-account/reservations")]
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
        }*/

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]

        [HttpGet("employee/offices")]
        public ActionResult<IEnumerable<Office>> GetAllOffices()
        {
            try
            {
                var offices = this._officeService.GetAllOffices();
                return Ok(offices.Offices);
                               
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("employee/office/image/{id}")]
        public ActionResult<OfficeResponse> ImageUrl(int id)
        {
           
            try{
                var office = _officeService.GetDetailsForOffice(id);
                if (office.OfficeImage != null)
                {
                    return Ok(office.OfficeImage);
                }
                else
                {
                    return BadRequest("Image not found");
                }
            }
            catch(Exception _)
            {
                return Unauthorized();
            }
        }

    }
}
