using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpPost("/authentication")]
        public ActionResult Authentication(MicrosoftUser user)
        {

            if (!user.Email.Contains("@it-labs.com"))
            {
                return BadRequest("Invalid email adress");
            }
            else if (_employeeRepository.GetByEmail(user.Email) != null)
            {
                return Ok("User allready exists, redirect depending on the role");
            }
            else
            {
                Employee microsoftuser = new Employee
                {
                    FirstName = user.Firstname,
                    LastName = user.Surname,
                    Email = user.Email,
                    JobTitle = user.JobTitle,
                    Password = BCrypt.Net.BCrypt.HashPassword("Passvord!23"),
                };
                _employeeRepository.Create(microsoftuser);

                return Ok("User created, redirect depending on the role");
            }
        }
    }
}
