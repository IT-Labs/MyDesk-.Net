using FluentValidation.Results;
using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IEmployeeService _employeeService;
        private readonly IConfiguration _configuration;

        public AuthController(IEmployeeService employeeRepository, IConfiguration configuration)
        {
            _employeeService = employeeRepository;
            _configuration = configuration;
        }

        [HttpPost("authentication")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Authentication([FromBody] EmployeeDto employeeDto)
        {
            return CreateEmployee(employeeDto);
        }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult Register([FromBody] EmployeeDto employeeDto)
        {
            return CreateEmployee(employeeDto);
        }

        [HttpPost("token")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetToken([FromBody] EmployeeDto employeeDto)
        {
            if (!_configuration.GetValue<bool>("Settings:UseCustomBearerToken") || string.IsNullOrEmpty(employeeDto.Email) || string.IsNullOrEmpty(employeeDto.Password))
            {
                return BadRequest();
            }

            byte[] data = Convert.FromBase64String(employeeDto.Password);
            string decodedPassword = Encoding.UTF8.GetString(data);

            Employee employee = _employeeService.GetByEmailAndPassword(employeeDto.Email, decodedPassword);
            if (employee == null)
            {
                return NotFound($"Employee with email: {employeeDto.Email} not found.");
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim("id", employee.Id.ToString()),
                new Claim("name", $"{employee.FirstName} {employee.LastName}"),
                new Claim("roles", RoleTypes.EMPLOYEE.ToString())
            });

            if (employee.IsAdmin == true)
            {
                claimsIdentity.AddClaim(new Claim("roles", RoleTypes.ADMIN.ToString()));
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_configuration["Settings:CustomBearerTokenSigningKey"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _configuration["JwtInfo:Issuer"],
                Audience = _configuration["JwtInfo:Audience"],
                Subject = claimsIdentity
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok($"Bearer {tokenHandler.WriteToken(token)}");
        }

        private IActionResult CreateEmployee(EmployeeDto employeeDto)
        {
            EmployeeDtoValidation validationRules = new EmployeeDtoValidation();
            ValidationResult validationResult = validationRules.Validate(employeeDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            Employee employee = _employeeService.GetByEmail(employeeDto.Email);
            if (employee != null)
            {
                return Ok("User allready exists, redirect depending on the role");
            }

            string password;
            bool isAdmin = false;

            // MS SSO
            if (string.IsNullOrEmpty(employeeDto.Password))
            {
                password = BCrypt.Net.BCrypt.HashPassword("Passvord!23");

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

            employee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                JobTitle = employeeDto.JobTitle,
                Password = password,
                IsAdmin = isAdmin
            };
            _employeeService.Create(employee);

            return Ok("User created, redirect depending on the role");
        }
    }
}
