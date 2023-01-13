using FluentValidation.Results;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.DTO;
using MyDesk.Data.Utils;
using MyDesk.Application.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Func<IEmployeeService> _employeeService;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(Func<IEmployeeService> employeeRepository, 
            IAuthService authService, 
            IConfiguration config)
        {
            _employeeService = employeeRepository;
            _authService = authService;
            _config = config;
        }

        [HttpPost("authentication")]
        public IActionResult Authentication([FromBody] EmployeeDto employeeDto)
        {
            if (_employeeService().GetByEmail(employeeDto?.Email ?? String.Empty) != null)
            {
                return Ok("User already exists, redirect depending on the role");
            }

            return CreateEmployee(employeeDto, true);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] EmployeeDto employeeDto)
        {
            return CreateEmployee(employeeDto, false);
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] EmployeeDto employeeDto)
        {
            if (string.IsNullOrEmpty(employeeDto.Email) || string.IsNullOrEmpty(employeeDto.Password))
            {
                return BadRequest();
            }

            byte[] data = Convert.FromBase64String(employeeDto.Password);
            string decodedPassword = Encoding.UTF8.GetString(data);

            EmployeeDto employee = _employeeService().GetByEmailAndPassword(employeeDto.Email, decodedPassword);
            if (employee.IsSSOAccount == true)
            {
                return BadRequest($"Employee with email address {employeeDto.Email} does not exist.");
            }
            
            string token = _authService.GetToken(employee,String.Empty);
            
            return Ok(token);
        }

        private IActionResult CreateEmployee(EmployeeDto employeeDto, bool isSSOAccount = false)
        {
            EmployeeDtoValidation validationRules = new();
            ValidationResult validationResult = validationRules.Validate(employeeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            if (_employeeService().GetByEmail(employeeDto?.Email ?? String.Empty) != null)
            {
                return BadRequest($"User with email address {employeeDto?.Email} already exists");
            }

            if (!isSSOAccount && string.IsNullOrEmpty(employeeDto?.Password??String.Empty))
            {
                return BadRequest("Password is not provided");
            }

            string password;
            bool isAdmin = false; 

            // SSO
            if (string.IsNullOrEmpty(employeeDto?.Password) && isSSOAccount)
            {
                password = BCrypt.Net.BCrypt.HashPassword(_config["AdminPassword"]);

                // Check if user is marked as admin
                string authHeader = Request.Headers[HeaderNames.Authorization];
                string jwt = authHeader[7..];
                JwtPayload jwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
                List<Claim> roles = jwtSecurityTokenDecoded.Claims.Where(x => x.Type == "roles").ToList();
                isAdmin = roles.Any(x => x.Value == RoleTypes.ADMIN.ToString());
            }
            // Custom log-in
            else
            {
                byte[] data = Convert.FromBase64String(employeeDto?.Password??String.Empty);
                string decodedPassword = Encoding.UTF8.GetString(data);

                password = BCrypt.Net.BCrypt.HashPassword(decodedPassword);
            }

            var employee = new EmployeeDto
            {
                FirstName = employeeDto?.FirstName,
                Surname = employeeDto?.Surname,
                Email = employeeDto?.Email,
                JobTitle = employeeDto?.JobTitle,
                Password = password,
                IsAdmin = isAdmin,
                IsSSOAccount = isSSOAccount
            };
            _employeeService().Create(employee);

            return Ok("User created, redirect depending on the role");
        }
    }
}
