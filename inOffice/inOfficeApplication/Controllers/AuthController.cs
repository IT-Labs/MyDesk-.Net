using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
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
        public string Register(RegisterDto dto)
        {
            try {
                var email = dto.Email;
                if (_employeeRepository.GetByEmail(email) == null)
                {
                    var employee = new Employee
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName,
                        Email = dto.Email,
                        JobTitle = dto.JobTitle,
                        Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    };
                    _employeeRepository.Create(employee);
                    return "Success";
                }
                else return "Email already exist!";
              
            } 
            catch(Exception _)
            {
                return "Email already exist!";
            }
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
 
        

    }
}
