using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using inOffice.Repository.Interface;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IOfficeService _officeService;
        private readonly JwtService _jwtService;

        public AdminController(IOfficeService officeService, JwtService jwtService)
        {
            _officeService = officeService;
            _jwtService = jwtService;
        }

        [HttpGet("admin/configuration")]
        public IActionResult Configuration()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    return Ok(admin);
                }
                else return Unauthorized();
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [HttpGet("admin/reservations")]
        public IActionResult Reservations()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    return Ok(admin);
                }
                else return Unauthorized();
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [HttpGet("admin/dashboard")]
        public IActionResult Admin()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];

                var admin = _jwtService.AdminRoleVerification(authHeader);

                if (admin != null)
                {
                    return Ok(admin);
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
