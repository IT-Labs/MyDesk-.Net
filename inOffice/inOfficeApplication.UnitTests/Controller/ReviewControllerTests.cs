using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Controllers;
using inOfficeApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Controller
{
    public class ReviewControllerTests
    {
        private ReviewController _reviewController;
        private IReviewService _reviewService;

        [OneTimeSetUp]
        public void Setup()
        {
            _reviewService = Substitute.For<IReviewService>();
            _reviewController = new ReviewController(_reviewService);
        }

        [Test]
        [Order(1)]
        public void ShowReview_Success()
        {
            // Arrange
            ReviewDto reviewDto = new ReviewDto() { Id = 1, Reviews = "Test review" };
            _reviewService.ShowReview(reviewDto.Id).Returns(reviewDto);

            // Act
            IActionResult result = _reviewController.ShowReview(reviewDto.Id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reviewDto);
        }

        [Test]
        [Order(2)]
        public void AllEntitiesForDesk_Success()
        {
            // Arrange
            int id = 11;
            List<ReviewDto> reviewDtos = new List<ReviewDto>() { new ReviewDto() { Id = 1, Reviews = "Test review" }, new ReviewDto() { Id = 2, Reviews = "review" } };
            _reviewService.GetReviewsForDesk(id).Returns(reviewDtos);

            // Act
            IActionResult result = _reviewController.AllEntitiesForDesk(id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reviewDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(3)]
        public void ReviewsAll_Success(int? take, int? skip)
        {
            // Arrange
            _reviewController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };

            List<ReviewDto> reviewDtos = new List<ReviewDto>() { new ReviewDto() { Id = 1, Reviews = "Test review" }, new ReviewDto() { Id = 2, Reviews = "review" } };
            _reviewService.AllReviews(take: take, skip: skip).Returns(reviewDtos);

            // Act
            IActionResult result = _reviewController.ReviewsAll();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reviewDtos);
        }

        [Test]
        [Order(4)]
        public void CreateReview_Success()
        {
            // Arrange
            ReviewDto reviewDto = new ReviewDto()
            {
                Reviews = "review",
                Reservation = new ReservationDto()
                {
                    Id = 1,
                    StartDate = DateTime.Now.AddDays(-1).Date,
                    EndDate = DateTime.Now.AddDays(1).Date,
                    Employee = new EmployeeDto(),
                    ConferenceRoom = new ConferenceRoomDto(),
                    Desk = new DeskDto(),
                    Reviews = new List<ReviewDto>()
                }
            };

            // Act
            IActionResult result = _reviewController.CreateReview(reviewDto);

            // Assert
            Assert.IsTrue(result is OkResult);
            _reviewService.Received(1).CreateReview(reviewDto);
            _reviewService.ClearReceivedCalls();
        }

        [TestCase(true)]
        [TestCase(false)]
        [Order(5)]
        public void CreateReview_ValidationFailed(bool reservationIsNull)
        {
            // Arrange
            ReviewDto reviewDto = new ReviewDto();
            if (!reservationIsNull)
            {
                reviewDto.Reservation = new ReservationDto();
            }

            // Act
            IActionResult result = _reviewController.CreateReview(reviewDto);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Review must contain text message.") && values.Any(x => x == "Review must be related to a reservation."));
            _reviewService.DidNotReceive().CreateReview(Arg.Any<ReviewDto>());
        }
    }
}
