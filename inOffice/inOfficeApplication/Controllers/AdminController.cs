using inOffice.BusinessLogicLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using inOffice.BusinessLogicLayer.Responses;
using Microsoft.AspNetCore.Cors;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Data.DTO;
using System.Net;

namespace inOfficeApplication.Controllers
{
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationDto<ReservationDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ReservationsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.AllReservations(take: take, skip: skip);

            return Ok(reservations);
        }

        [HttpGet("employee/reviews/all")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationDto<ReviewDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ReviewsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ReviewDto> reviews = _reviewService.AllReviews(take: take, skip: skip);

            return Ok(reviews);
        }
    }
}
