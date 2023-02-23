using MyDesk.Application.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;
using MyDesk.Core.Interfaces.BusinessLogic;
using MyDesk.Core.DTO;
using MyDesk.Core.Requests;

namespace MyDesk.UnitTests.Controller
{
    public class DeskControllerTests
    {
        private DeskController _deskController;
        private IDeskService _deskService;

        [OneTimeSetUp]
        public void Setup()
        {
            _deskService = Substitute.For<IDeskService>();
            _deskController = new DeskController(_deskService);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void GetAllDesksForEmployee_Success(int? take, int? skip)
        {
            // Arrange
            int id = 13;
            List<DeskDto> deskDtos = new List<DeskDto>() { new DeskDto() { Id = 1 }, new DeskDto() { Id = 2 } };

            _deskController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _deskService.GetOfficeDesks(id, take: take, skip: skip).Returns(deskDtos);

            // Act
            IActionResult result = _deskController.GetAllDesksForEmployee(id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == deskDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(2)]
        public void GetAllDesks_Success(int? take, int? skip)
        {
            // Arrange
            int id = 13;
            List<DeskDto> deskDtos = new List<DeskDto>() { new DeskDto() { Id = 1 }, new DeskDto() { Id = 2 } };

            _deskController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _deskService.GetOfficeDesks(id, take: take, skip: skip).Returns(deskDtos);

            // Act
            IActionResult result = _deskController.GetAllDesks(id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == deskDtos);
        }

        [Test]
        [Order(3)]
        public void Delete_Success()
        {
            // Arrange
            int id = 5;

            // Act
            IActionResult result = _deskController.Delete(id);

            // Assert
            Assert.IsTrue(result is OkResult);
            _deskService.Received(1).Delete(id);
            _deskService.ClearReceivedCalls();
        }

        [Test]
        [Order(4)]
        public void Create_Success()
        {
            // Arrange
            int id = 3;
            EntitiesRequest entitiesRequest = new EntitiesRequest() { NumberOfDesks = 5 };

            // Act
            IActionResult result = _deskController.Create(id, entitiesRequest);

            // Assert
            Assert.IsTrue(result is OkResult);
            _deskService.Received(1).Create(id, entitiesRequest.NumberOfDesks);
            _deskService.ClearReceivedCalls();
        }

        [TestCase(-2)]
        [TestCase(505)]
        [Order(5)]
        public void Create_ValidationFailed(int numberOfDesks)
        {
            // Arrange
            int id = 3;
            EntitiesRequest entitiesRequest = new EntitiesRequest() { NumberOfDesks = numberOfDesks };

            // Act
            IActionResult result = _deskController.Create(id, entitiesRequest);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Maximum number of desks to be created is 500.") || values.Any(x => x == "'Number Of Desks' must be greater than '0'."));
            _deskService.DidNotReceive().Create(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        [Order(6)]
        public void Update_Success()
        {
            // Arrange
            List<DeskDto> deskDtos = new List<DeskDto>() 
            { 
                new DeskDto() 
                { 
                    Id = 1,
                    IndexForOffice = 1,
                    Office = new OfficeDto(),
                    Category = new CategoryDto(),
                    Reservations = new List<ReservationDto>()
                }
            };

            // Act
            IActionResult result = _deskController.Update(deskDtos);

            // Assert
            Assert.IsTrue(result is OkResult);
            _deskService.Received(1).Update(deskDtos);
            _deskService.ClearReceivedCalls();
        }

        [Test]
        [Order(7)]
        public void Update_ValidationFailed()
        {
            // Arrange
            List<DeskDto> deskDtos = new List<DeskDto>() { new DeskDto() { } };

            // Act
            IActionResult result = _deskController.Update(deskDtos);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Desk needs to have an ID.") || values.Any(x => x == "Desk needs to have a category."));
            _deskService.DidNotReceive().Update(Arg.Any<List<DeskDto>>());
        }
    }
}
