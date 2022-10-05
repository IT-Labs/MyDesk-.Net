using inOffice.BusinessLogicLayer.Implementation;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using NUnit.Framework;

namespace inOfficeApplication.UnitTests.Service
{
    public class AuthServiceTests
    {
        private IAuthService _authService;
        private IApplicationParmeters _applicationParmeters;
        private IOpenIdConfigurationKeysFactory _openIdConfigurationKeysFactory;
        private IEmployeeRepository _employeeRepository;

        private string issuer = "it labs";
        private string audience = "app";
        private string signingKey = "1da503a4-138b-4c08-a7f1-618e7293eed5";

        [OneTimeSetUp]
        public void Setup()
        {
            _applicationParmeters = Substitute.For<IApplicationParmeters>();
            _openIdConfigurationKeysFactory = Substitute.For<IOpenIdConfigurationKeysFactory>();
            _employeeRepository = Substitute.For<IEmployeeRepository>();

            _applicationParmeters.GetJwtIssuer().Returns(issuer);
            _applicationParmeters.GetJwtAudience().Returns(audience);
            _applicationParmeters.GetCustomBearerTokenSigningKey().Returns(signingKey);

            _authService = new AuthService(_applicationParmeters, _openIdConfigurationKeysFactory, () => _employeeRepository);
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
                Surname = "Doe",
                Email = "john.doe@it-labs.com",
                IsAdmin = isAdmin
            };

            _employeeRepository.GetByEmail("john.doe@it-labs.com").Returns(new Employee());

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", AuthTypes.Custom));
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/admin/offices", "get", AuthTypes.Custom));

            if (isAdmin)
            {
                Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", AuthTypes.Custom));
            }
            else
            {
                Assert.IsFalse(_authService.ValidateToken(jwtToken, "/admin/office-desks/5", "post", AuthTypes.Custom));
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
                Surname = "Doe",
                Email = "john.doe@it-labs.com"
            };

            _employeeRepository.GetByEmail("john.doe@it-labs.com").Returns(new Employee());

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            Exception exception = Assert.Throws<Exception>(() => _authService.ValidateToken(jwtToken, "/employee/offices/12", "get", AuthTypes.Custom));
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
                Surname = "Doe",
                Email = "john.doe@it-labs.com"
            };


            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            SecurityTokenSignatureKeyNotFoundException exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _authService.ValidateToken(jwtToken, "/employee/offices", "get", AuthTypes.Azure));
            Assert.IsTrue(exception.Message == "IDX10500: Signature validation failed. No security keys were provided to validate the signature.");
        }

        [Test]
        [Order(4)]
        public void GetToken_ValidateToken_Custom_ThrowsSecurityTokenValidationException()
        {
            // Arrange
            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                FirstName = "John",
                Surname = "Doe",
                Email = "john.doe2@it-labs.com"
            };

            // Act
            string token = _authService.GetToken(employeeDto);

            // Assert
            string jwtToken = token.Substring(7);
            SecurityTokenValidationException exception = Assert.Throws<SecurityTokenValidationException>(() => _authService.ValidateToken(jwtToken, "/employee/offices/12", "get", AuthTypes.Custom));
            Assert.IsTrue(exception.Message == $"Employee with email {employeeDto.Email} was not found in DB.");
        }
    }
}