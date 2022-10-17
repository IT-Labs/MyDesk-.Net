using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Requests;
using inOfficeApplication.Controllers;
using inOfficeApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Controller
{
    public class ReservationControllerTests
    {
        private ReservationController _reservationController;
        private IReservationService _reservationService;

        [OneTimeSetUp]
        public void Setup()
        {
            _reservationService = Substitute.For<IReservationService>();
            _reservationController = new ReservationController(_reservationService);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void ReservationsAll_Success(int? take, int? skip)
        {
            // Arrange
            _reservationController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };

            PaginationDto<ReservationDto> reservationDtos = new PaginationDto<ReservationDto>()
            {
                TotalCount = 2,
                Values = new List<ReservationDto>() { new ReservationDto() { Id = 1 }, new ReservationDto() { Id = 2 } }
            };

            _reservationService.AllReservations(take: take, skip: skip).Returns(reservationDtos);

            // Act
            IActionResult result = _reservationController.ReservationsAll();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reservationDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(2)]
        public void EmployeeReservations_Success(int? take, int? skip)
        {
            // Arrange
            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 1 }, new ReservationDto() { Id = 2 } };

            _reservationController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _reservationService.FutureReservations("test@it-labs.com", take: take, skip: skip).Returns(reservationDtos);

            // Act
            IActionResult result = _reservationController.EmployeeReservations();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reservationDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(3)]
        public void PastReservations_Success(int? take, int? skip)
        {
            // Arrange
            List<ReservationDto> reservationDtos = new List<ReservationDto>() { new ReservationDto() { Id = 1 }, new ReservationDto() { Id = 2 } };

            _reservationController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _reservationService.PastReservations("test@it-labs.com", take: take, skip: skip).Returns(reservationDtos);

            // Act
            IActionResult result = _reservationController.PastReservations();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == reservationDtos);
        }

        [Test]
        [Order(4)]
        public void CancelReservation_Success()
        {
            // Arrange
            int id = 12;

            // Act
            IActionResult result = _reservationController.CancelReservation(id);

            // Assert
            Assert.IsTrue(result is OkResult);
            _reservationService.Received(1).CancelReservation(id);
        }

        [Test]
        [Order(5)]
        public void CoworkerReservation_Success()
        {
            // Arrange
            ReservationRequest reservationRequest = new ReservationRequest()
            {
                DeskId = 12,
                EmployeeEmail = "test email",
                StartDate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"),
                EndDate = DateTime.Now.AddDays(2).ToString("dd-MM-yyyy")
            };

            // Act
            IActionResult result = _reservationController.CoworkerReservation(reservationRequest);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value.ToString() == $"Sucessfuly reserved desk for coworker with mail {reservationRequest.EmployeeEmail}");
            _reservationService.Received(1).CoworkerReserve(reservationRequest);
            _reservationService.ClearReceivedCalls();
        }

        [Test]
        [Order(6)]
        public void CoworkerReservation_ValidationFailed()
        {
            // Arrange
            ReservationRequest reservationRequest = new ReservationRequest()
            {
                StartDate = DateTime.Now.AddDays(-2).ToString("dd-MM-yyyy"),
                EndDate = DateTime.Now.AddDays(-1).ToString("dd-MM-yyyy")
            };

            // Act
            IActionResult result = _reservationController.CoworkerReservation(reservationRequest);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Desk ID must be greater than zero.") && values.Any(x => x == "Employee email must have a value.") &&
                values.Any(x => x == "Reservation's start date must be in the future.") && values.Any(x => x == "Reservation's end date must be in the future."));
            _reservationService.DidNotReceive().CoworkerReserve(Arg.Any<ReservationRequest>());
        }
    }
}
