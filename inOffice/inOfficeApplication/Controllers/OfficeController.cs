using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class OfficeController : Controller
    {
        private readonly IOfficeService _officeService;
        private readonly IConfiguration _configuration;

        public OfficeController(IOfficeService officeService, IConfiguration configuration)
        {
            _officeService = officeService;
            _configuration = configuration;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPost("admin/office")]
        public ActionResult<OfficeResponse> AddNewOffice(NewOfficeRequest dto)
        {
            try
            {
                return Created("Success", _officeService.CreateNewOffice(dto));
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "EMPLOYEE")]
        [HttpGet("admin/office/image/{id}")]
        public ActionResult<OfficeResponse> ImageUrl(int id)
        {
            
            try { 
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
            catch
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpPut("admin/office/{id}")]
        public ActionResult<OfficeResponse> Edit(int id, OfficeRequest dto)
        {
            try
            {
                dto.Id = id;
                return Ok(_officeService.UpdateOffice(dto));   
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN")]
        [HttpDelete("admin/office/{id}")]
        public ActionResult<OfficeResponse> Delete(int id)
        {
           
            try
            {
                    if (id == null)
                    {
                        return NotFound();
                }
                else
                {
                    _officeService.DeleteOffice(id);
                    return Ok(new
                    {
                        message = "success"
                    });
                }
                    
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "EMPLOYEE")]
        [HttpGet("admin/offices")]
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
    }
}
