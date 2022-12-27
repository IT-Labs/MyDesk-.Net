using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Application.Controllers;
using MyDesk.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace MyDesk.UnitTests.Controller
{
    public class OfficeControllerTests
    {
        private OfficeController _officeController;
        private IOfficeService _officeService;

        [OneTimeSetUp]
        public void Setup()
        {
            _officeService = Substitute.For<IOfficeService>();
            _officeController = new OfficeController(_officeService);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void GetAllOfficesForEmployee_Success(int? take, int? skip)
        {
            // Arrange
            List<OfficeDto> officeDtos = new List<OfficeDto>() { new OfficeDto() { Id = 1 }, new OfficeDto() { Id = 2 } };

            _officeController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _officeService.GetAllOffices(take: take, skip: skip).Returns(officeDtos);

            // Act
            IActionResult result = _officeController.GetAllOfficesForEmployee();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == officeDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(2)]
        public void GetAllOffices_Success(int? take, int? skip)
        {
            // Arrange
            List<OfficeDto> officeDtos = new List<OfficeDto>() { new OfficeDto() { Id = 1 }, new OfficeDto() { Id = 2 } };

            _officeController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _officeService.GetAllOffices(take: take, skip: skip).Returns(officeDtos);

            // Act
            IActionResult result = _officeController.GetAllOffices();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == officeDtos);
        }

        [Test]
        [Order(3)]
        public void ImageUrlForEmployee_Success()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto() { Id = 1, OfficeImage = "test image" };
            _officeService.GetDetailsForOffice(officeDto.Id.Value).Returns(officeDto);

            // Act
            IActionResult result = _officeController.ImageUrlForEmployee(officeDto.Id.Value);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value.ToString() == officeDto.OfficeImage);
        }

        [Test]
        [Order(4)]
        public void ImageUrl_Success()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto() { Id = 1, OfficeImage = "test image" };
            _officeService.GetDetailsForOffice(officeDto.Id.Value).Returns(officeDto);

            // Act
            IActionResult result = _officeController.ImageUrl(officeDto.Id.Value);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value.ToString() == officeDto.OfficeImage);
        }

        [Test]
        [Order(5)]
        public void Delete_Success()
        {
            // Arrange
            int id = 5;

            // Act
            IActionResult result = _officeController.Delete(id);

            // Assert
            Assert.IsTrue(result is OkResult);
            _officeService.Received(1).DeleteOffice(id);
        }

        [Test]
        [Order(6)]
        public void AddNewOffice_Success()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto() { Name = "Test name" };

            // Act
            IActionResult result = _officeController.AddNewOffice(officeDto);

            // Assert
            Assert.IsTrue(result is CreatedResult);
            _officeService.Received(1).CreateNewOffice(officeDto);
            _officeService.ClearReceivedCalls();
        }

        [Test]
        [Order(7)]
        public void AddNewOffice_ValidationFailed()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto() { Id = -1 };

            // Act
            IActionResult result = _officeController.AddNewOffice(officeDto);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Office ID cannot be negative number.") && values.Any(x => x == "Office name cannot be empty."));
            _officeService.DidNotReceive().CreateNewOffice(Arg.Any<OfficeDto>());
        }

        [Test]
        [Order(8)]
        public void Edit_Success()
        {
            // Arrange
            int id = 3;
            OfficeDto officeDto = new OfficeDto() { Id = 2, Name = "Test name" };

            // Act
            IActionResult result = _officeController.Edit(id, officeDto);

            // Assert
            Assert.IsTrue(result is OkResult);
            _officeService.Received(1).UpdateOffice(Arg.Is<OfficeDto>(x => x.Id == id));
            _officeService.ClearReceivedCalls();
        }

        [Test]
        [Order(9)]
        public void Edit_ValidationFailed()
        {
            // Arrange
            OfficeDto officeDto = new OfficeDto() { Id = -1 };

            // Act
            IActionResult result = _officeController.Edit(officeDto.Id.Value, officeDto);

            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
            BadRequestObjectResult objectResult = (BadRequestObjectResult)result;
            Assert.NotNull(objectResult.Value);
            Assert.IsTrue(objectResult.Value is IEnumerable<string>);
            IEnumerable<string> values = (IEnumerable<string>)objectResult.Value;
            Assert.IsTrue(values.Any(x => x == "Office ID cannot be negative number.") && values.Any(x => x == "Office name cannot be empty."));
            _officeService.DidNotReceive().UpdateOffice(Arg.Any<OfficeDto>());
        }
    }
}
