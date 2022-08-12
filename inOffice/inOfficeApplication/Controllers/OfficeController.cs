using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class OfficeController : Controller
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpPost("admin/office")]
        public ActionResult<OfficeResponse> AddNewOffice(NewOfficeRequest dto)
        {
            try
            {
                OfficeResponse response = _officeService.CreateNewOffice(dto);
                if (response.Success != true)
                {
                    return Conflict("There is allready office with the same name");
                }
                else
                {
                    return Created("Success", response);
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.EmployeeRole)]
        [HttpGet("admin/office/image/{id}")]
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
            catch
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpPut("admin/office/{id}")]
        public ActionResult<OfficeResponse> Edit(int id, OfficeRequest dto)
        {
            try
            {
                dto.Id = id;
                OfficeResponse result = _officeService.UpdateOffice(dto);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AdminRole)]
        [HttpDelete("admin/office/{id}")]
        public ActionResult<OfficeResponse> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    return NotFound();
                }
                else
                {
                    OfficeResponse result = _officeService.DeleteOffice(id);
                    if (result.Success)
                    {
                        return Ok(new
                        {
                            message = "success"
                        });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.EmployeeRole)]
        [HttpGet("admin/offices")]
        public ActionResult<IEnumerable<Office>> GetAllOffices()
        {
            try
            {
                OfficeListResponse offices = _officeService.GetAllOffices();

                if (offices.Success)
                {
                    return Ok(offices.Offices);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
    }
}
