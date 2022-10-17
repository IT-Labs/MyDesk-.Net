using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Controllers;
using inOfficeApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Controller
{
    public class ConferenceRoomControllerTests
    {
        private ConferenceRoomController _conferenceRoomController;
        private IConferenceRoomService _conferenceRoomService;

        [OneTimeSetUp]
        public void Setup()
        {
            _conferenceRoomService = Substitute.For<IConferenceRoomService>();
            _conferenceRoomController = new ConferenceRoomController(_conferenceRoomService);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void GetAllConferenceRoomsForEmployee_Success(int? take, int? skip)
        {
            // Arrange
            int id = 13;
            List<ConferenceRoomDto> conferenceRoomDtos = new List<ConferenceRoomDto>() { new ConferenceRoomDto() { Id = 1 }, new ConferenceRoomDto() { Id = 2 } };

            _conferenceRoomController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip).Returns(conferenceRoomDtos);

            // Act
            IActionResult result = _conferenceRoomController.GetAllConferenceRoomsForEmployee(id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == conferenceRoomDtos);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(2)]
        public void GetAllConferenceRooms_Success(int? take, int? skip)
        {
            // Arrange
            int id = 6;
            List<ConferenceRoomDto> conferenceRoomDtos = new List<ConferenceRoomDto>() { new ConferenceRoomDto() { Id = 1 }, new ConferenceRoomDto() { Id = 2 } };

            _conferenceRoomController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _conferenceRoomService.GetOfficeConferenceRooms(id, take: take, skip: skip).Returns(conferenceRoomDtos);

            // Act
            IActionResult result = _conferenceRoomController.GetAllConferenceRooms(id);

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == conferenceRoomDtos);
        }

        [Test]
        [Order(3)]
        public void Delete_Success()
        {
            // Arrange
            int id = 4;

            // Act
            IActionResult result = _conferenceRoomController.Delete(id);

            // Assert
            Assert.IsTrue(result is OkResult);
            _conferenceRoomService.Received(1).Delete(id);
        }
    }
}
