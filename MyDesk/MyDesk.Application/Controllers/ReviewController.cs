using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation.Results;
using MyDesk.Application.Validations;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.BusinessLogicLayer.Utils;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    [Authorize]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("employee/review/{id}")]
        public IActionResult ShowReview(int id)
        {
            ReviewDto review = _reviewService.ShowReview(id);
            return Ok(review);
        }

        [HttpGet("entity/reviews/{id}")]
        public IActionResult AllEntitiesForDesk(int id)
        {
            List<ReviewDto> reviews = _reviewService.GetReviewsForDesk(id);
            return Ok(reviews);
        }

        [HttpGet("employee/reviews/all")]
        public IActionResult ReviewsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<ReviewDto> reviews = _reviewService.AllReviews(take: take, skip: skip);

            return Ok(reviews);
        }

        [HttpPost("employee/review")]
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