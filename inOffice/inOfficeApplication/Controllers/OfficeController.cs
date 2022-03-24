using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class OfficeController : Controller
    {
        private readonly IOfficeService _officeService;
        private readonly JwtService _jwtService;

        public OfficeController(IOfficeService officeService, JwtService jwtService)
        {
            _officeService = officeService;
            _jwtService = jwtService;
        }
        //admin/addoffice
        [HttpPost("admin/office")]
        public ActionResult<OfficeResponse> AddNewOffice(NewOfficeRequest dto)
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];
                var admin = _jwtService.AdminRoleVerification(authHeader);
                
                if (admin != null)
                {
                    return Created("Success", _officeService.CreateNewOffice(dto));

                }
                else return Unauthorized();

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        //admin/office/image/id
        [HttpGet("admin/office/image/{id}")]
        public ActionResult<OfficeResponse> ImageUrl(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var admin = _jwtService.AdminRoleVerification(authHeader);
            if(admin != null)
            {
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
            else
            {
                return Unauthorized();
            }
        }
        //admin/edit/id
        [HttpPut("admin/office/{id}")]
        public ActionResult<OfficeResponse> Edit(int id, OfficeRequest dto)
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];
                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    dto.Id = id;
                    return Ok(_officeService.UpdateOffice(dto));
                }
                else return Unauthorized();
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        //HttpGet: admin/delete/id
        [HttpDelete("admin/office/{id}")]
        public ActionResult<OfficeResponse> Delete(int id)
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];
                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    if (id == null)
                    {
                        return NotFound();
                    }
                    _officeService.DeleteOffice(id);

                    return Ok(new
                    {
                        message = "success"
                    });
                }
                else return Unauthorized();
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        //admin/getalloffices
        [HttpGet("admin/offices")]
        public ActionResult<IEnumerable<Office>> GetAllOffices()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    var offices = this._officeService.GetAllOffices(); 
                    return Ok(offices.Offices);
                }
                else return Unauthorized();
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
    }
}
