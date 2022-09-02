using FluentValidation.Results;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("employee/review/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReviewDto))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ShowReview(int id)
        {
            ReviewDto review = _reviewService.ShowReview(id);
            return Ok(review);
        }

        [HttpGet("entity/reviews/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ReviewDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AllEntitiesForDesk(int id)
        {
            List<ReviewDto> reviews = _reviewService.GetReviewsForDesk(id);
            return Ok(reviews);
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

        [HttpPost("employee/review")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CreateReview([FromBody] ReviewDto reviewDto)
        {
            ReviewDtoValidation validationRules = new ReviewDtoValidation();
            ValidationResult validationResult = validationRules.Validate(reviewDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _reviewService.CreateReview(reviewDto);
            return Ok();
        }
    }
}