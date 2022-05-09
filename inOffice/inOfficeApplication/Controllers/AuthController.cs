using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authorization;
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
       

        public AuthController(IAdminRepository adminRepository, IEmployeeRepository employeeRepository)
        {
            _adminRepository = adminRepository; 
            _employeeRepository = employeeRepository;  
        }

        [AllowAnonymous]
        [HttpPost("/authentication")]
        public string Authentication(MicrosoftUser user)
        {

            if (_employeeRepository.GetByEmail(user.Email) != null)
            {
                return "User exist";
            }

            var microsoftuser = new Employee
            {
                FirstName = user.Firstname,
                LastName = user.Surname,
                Email = user.Email,
                JobTitle = user.JobTitle,
                Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
            };
            _employeeRepository.Create(microsoftuser);

            return "Oke";
        }

        /*[HttpPost("/register")]
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
                return "Bad Request!";
            }
        }*/
    }
}
