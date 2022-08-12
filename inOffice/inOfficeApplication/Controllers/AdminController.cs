using inOffice.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using inOffice.BusinessLogicLayer.Responses;
using Microsoft.AspNetCore.Cors;
using inOfficeApplication.Data.Utils;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IReservationService _reservationService;

        public AdminController(IReviewService reviewService, IReservationService reservationService)
        {
            _reviewService = reviewService;
            _reservationService = reservationService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/reservations/all")]
        public ActionResult<AllReservationsResponse> ReservationsAll()
        {
            try
            {
                AllReservationsResponse reservations = _reservationService.AllReservations();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [EnableCors]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/reviews/all")]
        public ActionResult<AllReviewsResponse> ReviewsAll()
        {
            try
            {
                AllReviewsResponse reviews = _reviewService.AllReviews();

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
