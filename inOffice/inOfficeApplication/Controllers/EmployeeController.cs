using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/offices")]
        public ActionResult<IEnumerable<Office>> GetAllOffices()
        {
            try
            {
                OfficeListResponse offices = _officeService.GetAllOffices();
                return Ok(offices.Offices);

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/office/image/{id}")]
        public ActionResult<OfficeResponse> ImageUrl(int id)
        {
            try
            {
                Office office = _officeService.GetDetailsForOffice(id);
                if (office.OfficeImage != null)
                {
                    return Ok(office.OfficeImage);
                }
                else
                {
                    return BadRequest("Image not found");
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

    }
}
