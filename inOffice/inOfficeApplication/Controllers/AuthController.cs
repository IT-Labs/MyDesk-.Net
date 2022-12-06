using FluentValidation.Results;
using inOfficeApplication.Data.Interfaces.BusinessLogic;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Func<IEmployeeService> _employeeService;
        private readonly IAuthService _authService;
        private readonly IApplicationParmeters _applicationParmeters;
        private readonly IConfiguration _configuration;

        public AuthController(Func<IEmployeeService> employeeRepository, 
            IAuthService authService, 
            IApplicationParmeters applicationParmeters,
            IConfiguration configuration)
        {
            _employeeService = employeeRepository;
            _authService = authService;
            _applicationParmeters = applicationParmeters;
            _configuration = configuration;
        }

        [HttpPost("authentication")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Authentication([FromBody] EmployeeDto employeeDto)
        {
            return CreateEmployee(employeeDto, true);
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Register([FromBody] EmployeeDto employeeDto)
        {
            return CreateEmployee(employeeDto, false);
        }

        [HttpPost("token")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetToken([FromBody] EmployeeDto employeeDto)
        {
            if (string.IsNullOrEmpty(employeeDto.Email) || string.IsNullOrEmpty(employeeDto.Password))
            {
                return BadRequest();
            }

            byte[] data = Convert.FromBase64String(employeeDto.Password);
            string decodedPassword = Encoding.UTF8.GetString(data);

            string headerTenant = Request.Headers["tenant"];
            if (!string.IsNullOrEmpty(headerTenant))
            {
                Dictionary<string, string> tenants = _applicationParmeters.GetTenants();

                if (tenants.ContainsKey(headerTenant))
                {
                    Request.HttpContext.Items["tenant"] = tenants[headerTenant];
                }
                else
                {
                    return BadRequest($"Tenant {headerTenant} does not exist.");
                }
            }

            EmployeeDto employee = _employeeService().GetByEmailAndPassword(employeeDto.Email, decodedPassword);
            string token = _authService.GetToken(employee, headerTenant);
            
            return Ok(token);
        }

        private IActionResult CreateEmployee(EmployeeDto employeeDto, bool isSSOAccount = false)
        {
            EmployeeDtoValidation validationRules = new EmployeeDtoValidation();
            ValidationResult validationResult = validationRules.Validate(employeeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            var employee = _employeeService().GetByEmail(employeeDto.Email);
            if (employee != null)
            {
                return Ok("User already exists, redirect depending on the role");
            }

            if (!isSSOAccount && string.IsNullOrEmpty(employeeDto.Password))
            {
                return BadRequest("Password is not provided");
            }

            string password;
            bool isAdmin = false; 

            // SSO
            if (string.IsNullOrEmpty(employeeDto.Password) && isSSOAccount)
            {
                password = BCrypt.Net.BCrypt.HashPassword(_configuration["AdminPassword"] ?? "");

                // Check if user is marked as admin
                string authHeader = Request.Headers[HeaderNames.Authorization];
                string jwt = authHeader.Substring(7);
                JwtPayload jwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
                List<Claim> roles = jwtSecurityTokenDecoded.Claims.Where(x => x.Type == "roles").ToList();
                isAdmin = roles.Any(x => x.Value == RoleTypes.ADMIN.ToString());
            }
            // Custom log-in
            else
            {
                byte[] data = Convert.FromBase64String(employeeDto.Password);
                string decodedPassword = Encoding.UTF8.GetString(data);

                password = BCrypt.Net.BCrypt.HashPassword(decodedPassword);
            }

            employee = new EmployeeDto
            {
                FirstName = employeeDto.FirstName,
                Surname = employeeDto.Surname,
                Email = employeeDto.Email,
                JobTitle = employeeDto.JobTitle,
                Password = password,
                IsAdmin = isAdmin,
                IsSSOAccount = isSSOAccount
            };
            _employeeService().Create(employee);

            return Ok("User created, redirect depending on the role");
        }
    }
}
