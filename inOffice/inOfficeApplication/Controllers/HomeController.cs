using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IReservationService _reservationService;

        public HomeController(JwtService jwtService, IReservationService reservationService)
        {
            _jwtService = jwtService;
            _reservationService = reservationService;
        }

        [HttpPost("employee/reserve")]
        public ActionResult<ReservationResponse> Reservation(ReservationRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);
            
            try
            {                
                if (employee != null)
                {
                    
                    var response =_reservationService.Reserve(dto, employee);
                    if (response.Success == true)
                    {
                        return Ok();
                    }
                    else
                    {
                       return BadRequest();
                    }
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
