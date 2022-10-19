using inOffice.BusinessLogicLayer;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.Interfaces.Repository;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Exceptions;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Http;
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
        private IEmployeeService _employeeService;
        private IHttpContextAccessor _httpContextAccessor;

        private string issuer = "it labs";
        private string audience = "app";
        private string signingKey = "1da503a4-138b-4c08-a7f1-618e7293eed5";

        [OneTimeSetUp]
        public void Setup()
        {
            _applicationParmeters = Substitute.For<IApplicationParmeters>();
            _openIdConfigurationKeysFactory = Substitute.For<IOpenIdConfigurationKeysFactory>();
            _employeeService = Substitute.For<IEmployeeService>();
            _httpContextAccessor = Substitute.For<IHttpContextAccessor>();

            _applicationParmeters.GetJwtIssuer().Returns(issuer);
            _applicationParmeters.GetJwtAudience().Returns(audience);
            _applicationParmeters.GetCustomBearerTokenSigningKey().Returns(signingKey);

            _authService = new AuthService(_applicationParmeters, _openIdConfigurationKeysFactory, () => _employeeService, _httpContextAccessor);
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

            _employeeService.GetByEmail("john.doe@it-labs.com").Returns(employeeDto);

            // Act
            string token = _authService.GetToken(employeeDto, string.Empty);

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

            _employeeService.GetByEmail("john.doe@it-labs.com").Returns(employeeDto);

            // Act
            string token = _authService.GetToken(employeeDto, string.Empty);

            // Assert
            string jwtToken = token.Substring(7);
            NotFoundException exception = Assert.Throws<NotFoundException>(() => _authService.ValidateToken(jwtToken, "/employee/offices/12", "get", AuthTypes.Custom));
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
            string token = _authService.GetToken(employeeDto, string.Empty);

            // Assert
            string jwtToken = token.Substring(7);
            SecurityTokenSignatureKeyNotFoundException exception = Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() => _authService.ValidateToken(jwtToken, "/employee/offices", "get", AuthTypes.Azure));
            Assert.IsTrue(exception.Message == "IDX10500: Signature validation failed. No security keys were provided to validate the signature.");
        }

        [Test]
        [Order(4)]
        public void GetToken_ValidateToken_Custom_Tenant_Success()
        {
            // Arrange
            string tenantClaimName = "tenant claim";
            Dictionary<string, string> tenants = new Dictionary<string, string>() { { "tenant name", "tenant connection string" } };

            EmployeeDto employeeDto = new EmployeeDto()
            {
                Id = 1,
                FirstName = "John",
                Surname = "Doe",
                Email = "john.doe@it-labs.com"
            };
            DefaultHttpContext defaultHttpContext = new DefaultHttpContext();

            _httpContextAccessor.HttpContext.Returns(defaultHttpContext);
            _applicationParmeters.GetTenantClaimKey().Returns(tenantClaimName);
            _applicationParmeters.GetTenants().Returns(tenants);
            _employeeService.GetByEmail("john.doe@it-labs.com").Returns(employeeDto);

            // Act
            string token = _authService.GetToken(employeeDto, tenants.First().Key);

            // Assert
            string jwtToken = token.Substring(7);
            Assert.IsTrue(_authService.ValidateToken(jwtToken, "/employee/offices", "get", AuthTypes.Custom));
            Assert.IsTrue(defaultHttpContext.Items["tenant"] == tenants.First().Value);
        }
    }
}