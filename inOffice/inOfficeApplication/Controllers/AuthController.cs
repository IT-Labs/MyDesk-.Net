using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly JwtService _jwtService;
       

        public AuthController(IAdminRepository adminRepository, JwtService jwtService, IEmployeeRepository employeeRepository)
        {
            _adminRepository = adminRepository; 
            _jwtService = jwtService;   
            _employeeRepository = employeeRepository;  
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            return Created("Success", _employeeRepository.Create(employee));

        }

        [HttpPost("/login")]
        public string Login(LoginDto dto)
        {
            var admin = _adminRepository.GetByEmail(dto.Email);
            var employee = _employeeRepository.GetByEmail(dto.Email);

            if (admin == null && employee==null)
            {
                 return "Invalid credentials";  
            }

            if (admin != null) { 

                if (BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
                {
                    return _jwtService.Generate(admin.Id, "ADMIN");
                }
                else
                {
                    return "Invalid Credentials";
                }
            }
            if (employee != null)
            {
                if (BCrypt.Net.BCrypt.Verify(dto.Password, employee.Password))
                {
                    return _jwtService.Generate(employee.Id, "EMPLOYEE");
                }
                else
                {
                    return "Invalid Credentials";
                }
            }

            return "Invalid credentials";
 
        }
 
        [HttpGet("admin/dashboard")]
        public IActionResult Admin()
        {
            try
            {
                //var jwt = Request.Cookies["jwt"];
                string authHeader = Request.Headers[HeaderNames.Authorization];
                var jwt = authHeader.Substring(7);
                //Console.WriteLine("Error"+jwt);
                var token = _jwtService.Verify(jwt);

                int adminId = int.Parse(token.Issuer);

                var admin = _adminRepository.GetById(adminId);

                return Ok(admin);
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        
        [HttpGet("employee/home")]
        public IActionResult Employee()
        {
            try
            {
                string authHeader = Request.Headers[HeaderNames.Authorization];
                var jwt = authHeader.Substring(7);
                var token = _jwtService.Verify(jwt);
                int employeeId = int.Parse(token.Issuer);
                var employee = _employeeRepository.GetById(employeeId);

                return Ok(employee);
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

    }
}
