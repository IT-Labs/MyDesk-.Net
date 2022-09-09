using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Controllers;
using inOfficeApplication.Data.DTO;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Controller
{
    public class EmployeeControllerTests
    {
        private EmployeeController _employeeController;
        private IEmployeeService _employeeService;

        [OneTimeSetUp]
        public void Setup()
        {
            _employeeService = Substitute.For<IEmployeeService>();
            _employeeController = new EmployeeController(_employeeService);
        }

        [TestCase(null, null)]
        [TestCase(10, 0)]
        [Order(1)]
        public void AllEmployees_Success(int? take, int? skip)
        {
            // Arrange
            List<EmployeeDto> employeeDtos = new List<EmployeeDto>() { new EmployeeDto() { Id = 1 }, new EmployeeDto() { Id = 2 } };

            _employeeController.ControllerContext = new ControllerContext() { HttpContext = ControllerTestHelper.GetMockedHttpContext(take: take, skip: skip) };
            _employeeService.GetAll(take: take, skip: skip).Returns(employeeDtos);

            // Act
            IActionResult result = _employeeController.AllEmployees();

            // Assert
            Assert.IsTrue(result is OkObjectResult);
            OkObjectResult objectResult = (OkObjectResult)result;
            Assert.IsTrue(objectResult.Value == employeeDtos);
        }

        [Test]
        [Order(2)]
        public void SetAsAdmin_Success()
        {
            // Arrange
            int id = 7;

            // Act
            IActionResult result = _employeeController.SetAsAdmin(id);

            // Assert
            Assert.IsTrue(result is OkResult);
            _employeeService.Received(1).SetEmployeeAsAdmin(id);
        }
    }
}
