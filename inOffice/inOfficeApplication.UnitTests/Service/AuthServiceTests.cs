using inOffice.BusinessLogicLayer.Implementation;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private IApplicationParmeters _applicationParmeters;

        private string issuer = "it labs";
        private string audience = "app";
        private string signingKey = "1da503a4-138b-4c08-a7f1-618e7293eed5";

        [OneTimeSetUp]
        public void Setup()
        {
            _applicationParmeters = Substitute.For<IApplicationParmeters>();
            _applicationParmeters.GetJwtIssuer().Returns(issuer);
            _applicationParmeters.GetJwtAudience().Returns(audience);
            _applicationParmeters.GetCustomBearerTokenSigningKey().Returns(signingKey);

            _authService = new AuthService(_applicationParmeters);
        }

        [TestCase(false)]
        [TestCase(true)]
        [Order(1)]
        public void GetToken_ValidateToken_Custom_Success(bool isAdmin)
        {
            // Arrange
            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@it-labs.com",
                IsAdmin = isAdmin
            };

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", useCustomLogin: true, null));
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/admin/offices", "get", useCustomLogin: true, null));

            if (isAdmin)
            {
                Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", useCustomLogin: true, null));
            }
            else
            {
                Assert.IsFalse(_authService.ValidateToken(jwtToken, "/admin/office-desks/5", "post", useCustomLogin: true, null));
            }
        }

        [Test]
        [Order(2)]
        public void GetToken_ValidateToken_Custom_ThrowsException()
        {
            // Arrange
            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@it-labs.com"
            };

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Exception exception = Assert.Throws<Exception>(() => _authService.ValidateToken(jwtToken, "/employee/offices/12", "get", useCustomLogin: true, null));
            Assert.IsTrue(exception.Message == "Required endpoint doesn't exist.");
        }

        [Test]
        [Order(3)]
        public void GetToken_ValidateToken_MS_SSO_ThrowsException()
        {
            // Arrange
            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@it-labs.com"
            };


            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            SecurityTokenSignatureKeyNotFoundException exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _authService.ValidateToken(jwtToken, "/employee/offices", "get", useCustomLogin: false, null));
            Assert.IsTrue(exception.Message == "IDX10500: Signature validation failed. No security keys were provided to validate the signature.");
        }
    }
}