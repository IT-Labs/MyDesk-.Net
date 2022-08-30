﻿using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IConfiguration _configuration;

        public AuthController(IEmployeeService employeeRepository, IConfiguration configuration)
        {
            _employeeService = employeeRepository;
            _configuration = configuration;
        }

        [HttpPost("authentication")]
        public IActionResult Authentication(MicrosoftUser user)
        {
            return CreateEmployee(user);
        }

        [HttpPost("register")]
        public IActionResult Register(MicrosoftUser user)
        {
            user.Email = user.Email.Trim();
            if (user.Email.Length < 3 || user.Email.Length > 254)
            {
                return BadRequest("Email length should be between 3 and 254.");
            }

            return CreateEmployee(user);
        }

        [HttpPost("token")]
        public IActionResult GetToken(MicrosoftUser user)
        {
            if (!_configuration.GetValue<bool>("Settings:UseCustomBearerToken") || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest();
            }

            byte[] data = Convert.FromBase64String(user.Password);
            string decodedPassword = Encoding.UTF8.GetString(data);

            Employee employee = _employeeService.GetByEmailAndPassword(user.Email, decodedPassword);
            if (employee == null)
            {
                return NotFound();
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

        private IActionResult CreateEmployee(MicrosoftUser user)
        {
            if (!user.Email.Contains("@it-labs.com"))
            {
                return BadRequest("Invalid email adress");
            }
            else if (_employeeService.GetByEmail(user.Email) != null)
            {
                return Ok("User allready exists, redirect depending on the role");
            }
            else
            {
                string password;
                bool isAdmin = false;

                // MS SSO
                if (string.IsNullOrEmpty(user.Password))
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
                    byte[] data = Convert.FromBase64String(user.Password);
                    string decodedPassword = Encoding.UTF8.GetString(data);

                    password = BCrypt.Net.BCrypt.HashPassword(decodedPassword);
                }


                Employee microsoftuser = new Employee
                {
                    FirstName = user.Firstname,
                    LastName = user.Surname,
                    Email = user.Email,
                    JobTitle = user.JobTitle,
                    Password = password,
                    IsAdmin = isAdmin
                };
                _employeeService.Create(microsoftuser);

                return Ok("User created, redirect depending on the role");
            }
        }
    }
}
