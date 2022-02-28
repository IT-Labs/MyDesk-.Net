using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Helpers;
using inOfficeApplication.Models;
using Microsoft.AspNetCore.Http;
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

            //_adminRepository.Create(admin);

            return Created("Success", _employeeRepository.Create(employee));

        }

        [HttpPost("/admin/register")]
        public IActionResult AddAdmin(RegisterDto dto)
        {
            var admin = new Admin
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            //_adminRepository.Create(admin);

            return Created("Success", _adminRepository.Create(admin));

        }
/*
        [HttpPost("/test/login")]
        public IActionResult LoginTest(LoginDto dto)
        {
            var employee = _employeeRepository.GetByEmail(dto.Email);
            if (employee == null)
            {
                return BadRequest();
            }
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, employee.Password))
            {
                return BadRequest();
            }
            return Ok(new
            {
                message = "success"
            });

        }*/

        [HttpPost("login")]
        public string Login(LoginDto dto)
        {
            var admin = _adminRepository.GetByEmail(dto.Email);
            var employee = _employeeRepository.GetByEmail(dto.Email);
            
            var s= "";

            if (admin == null)
            {
                
                 //return Json(new { Error = "Invalid Credentials" });
                 //return "Invalid credentials";
                 s= _jwtService.Generate(employee.Id,"EMPLOYEE");
                 return s ;
            }


            if (!BCrypt.Net.BCrypt.Verify(dto.Password, admin.Password))
            {
                return "Invalid credentials";
            }
          /*  if (!BCrypt.Net.BCrypt.Verify(dto.Password, employee.Password))
            {
                return "Invalid credentials";
            }
*/

            var jwt = _jwtService.Generate(admin.Id,"ADMIN");
            
            return jwt;

           /* return Ok(new
            {
                message="success"
            });*/ 
        }



        [HttpGet("admin")]
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

        [HttpGet("admin/myaccount")]
        public string TestFunc()
        {
            return "Shto i da bilo";
        }

        [HttpGet("employee/home")]
        public string funcTest2()
        {
            return "Shto i da bilo2";
        }

        /* [HttpPost("logout")]
         public IActionResult Logout()
         {

             Response.Cookies.Delete("jwt");
             return Ok(new {

                 message="Success"
             });

         }*/

    }
}
