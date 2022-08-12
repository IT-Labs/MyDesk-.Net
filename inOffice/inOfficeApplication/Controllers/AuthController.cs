using inOffice.BusinessLogicLayer.Requests;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inOfficeApplication.Controllers
{

    [Route("")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;

        public AuthController(IEmployeeRepository employeeRepository)
        {
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
