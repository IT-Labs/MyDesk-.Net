using inOffice.BusinessLogicLayer.Implementation;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private IConfiguration _configuration;

        private string issuer = "it labs";
        private string audience = "app";
        private string signingKey = "1da503a4-138b-4c08-a7f1-618e7293eed5";

        [OneTimeSetUp]
        public void Setup()
        {
            _configuration = Substitute.For<IConfiguration>();
            _configuration[Arg.Is<string>(x => x == "JwtInfo:Issuer")].Returns(issuer);
            _configuration[Arg.Is<string>(x => x == "JwtInfo:Audience")].Returns(audience);
            _configuration[Arg.Is<string>(x => x == "Settings:CustomBearerTokenSigningKey")].Returns(signingKey);

            _authService = new AuthService(_configuration);
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

            _configuration[Arg.Is<string>(x => x == "Settings:UseCustomBearerToken")].Returns("true");

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", null));
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/admin/offices", "get", null));

            if (isAdmin)
            {
                Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", null));
            }
            else
            {
                Assert.IsFalse(_authService.ValidateToken(jwtToken, "/admin/office-desks/5", "post", null));
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

            _configuration[Arg.Is<string>(x => x == "Settings:UseCustomBearerToken")].Returns("true");

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Exception exception = Assert.Throws<Exception>(() => _authService.ValidateToken(jwtToken, "/employee/offices/12", "get", null));
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

            _configuration[Arg.Is<string>(x => x == "Settings:UseCustomBearerToken")].Returns("false");

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            SecurityTokenSignatureKeyNotFoundException exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _authService.ValidateToken(jwtToken, "/employee/offices", "get", null));
            Assert.IsTrue(exception.Message == "IDX10500: Signature validation failed. No security keys were provided to validate the signature.");
        }
    }
}
