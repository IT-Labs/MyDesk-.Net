using inOffice.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("employee/reservations/all")]
        public ActionResult<AllReservationsResponse> ReservationsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            AllReservationsResponse reservations = _reservationService.AllReservations(take: take, skip: skip);

            if (reservations.Success != true)
            {
                return BadRequest();
            }
            else
            {
                return Ok(reservations);
            }
        }

        [EnableCors]
        [HttpGet("employee/reviews/all")]
        public ActionResult<AllReviewsResponse> ReviewsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            AllReviewsResponse reviews = _reviewService.AllReviews(take: take, skip: skip);

            if (reviews.Success != true)
            {
                return BadRequest();
            }
            else
            {
                return Ok(reviews);
            }
        }
    }
}
