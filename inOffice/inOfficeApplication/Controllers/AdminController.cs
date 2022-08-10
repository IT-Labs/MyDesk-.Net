using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using inOffice.Repository.Interface;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inOffice.BusinessLogicLayer.Responses;
using Microsoft.AspNetCore.Cors;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IOfficeService _officeService;
        private readonly IReservationService _reservationService;

        public AdminController(IOfficeService officeService, IReservationService reservationService)
        {
            _officeService = officeService;
            _reservationService = reservationService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("employee/reservations/all")]
        public ActionResult<AllReservationsResponse> ReservationsAll()
        {
            try
            {
                var reservations = _reservationService.AllReservations();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [EnableCors]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("employee/reviews/all")]
        public ActionResult<AllReviewsResponse> ReviewsAll()
        {
            try
            {

                var reviews = _reservationService.AllReviews();

                if (reviews.Success != true)
                {
                    return BadRequest();
                }
                else
                {
                    return Ok(reviews);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
