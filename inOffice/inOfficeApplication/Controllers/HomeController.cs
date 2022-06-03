using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOffice.Repository.Interface;
using inOfficeApplication.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IEmployeeRepository _employeeRepository;


        public HomeController(IReservationService reservationService, IEmployeeRepository employeeRepository)
        {
            _reservationService = reservationService;
            _employeeRepository = employeeRepository;
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpPost("employee/reserve")]
        public ActionResult<ReservationResponse> Reservation(ReservationRequest dto)
        {

            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email); ;

            try
            {

                var response = _reservationService.Reserve(dto, employee);
                if (response.Success == true)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpPost("employee/reserve/coworker")]
        public ActionResult<ReservationResponse> CoworkerReservation(CoworkerReservationRequest dto)
        {
            try
            {
                var response = _reservationService.CoworkerReserve(dto);
                if(response.Success == true)
                {
                    string msg = String.Format("Sucessfuly reserved desk for coworker with mail {0}", dto.CoworkerMail);
                    return Ok(msg);
                }
                else
                {
                    string error = String.Format("Reservation for that time period allready exists for {0}", dto.CoworkerMail);
                    return Conflict(error);
                }
            }
            catch(Exception _)
            {
                return BadRequest();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("employee/all")]
        public ActionResult<List<CustomEmployee>> AllEmployees()
        {
            try
            {
                var employees = _employeeRepository.GetAll().ToList();
                List<CustomEmployee> result = new List<CustomEmployee>();

               foreach (var employee in employees)
               {
                    result.Add(new CustomEmployee(employee.Id, employee.FirstName, employee.LastName, employee.Email, employee.JobTitle));
               }

               var filtereResult = result.DistinctBy(x=> x.Email);

               return Ok(filtereResult);

            }catch (Exception _)
            {
                return NotFound();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("employee/future-reservation")]
        public ActionResult<EmployeeReservationsResponse> EmployeeReservations()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email); ;

            try
            {
                if (employee != null)
                {

                    var response = _reservationService.EmployeeReservations(employee);

                    if (response.Success == true)
                    {
                        return Ok(response.CustomReservationResponses);
                    }
                    else
                    {
                        return BadRequest("Sucess is false");
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]

        [HttpGet("employee/past-reservations")]
        public ActionResult<EmployeeReservationsResponse> PastReservations()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email);

            try
            {
                if (employee != null)
                {

                    var response = _reservationService.PastReservations(employee);

                    if (response.Success == true)
                    {
                        return Ok(response.CustomReservationResponses);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]

        [HttpGet("employee/review/{id}")]
        public ActionResult<ReviewResponse> ShowReview(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email);

            try
            {
                if(employee != null)
                {
                    var ReviewForGivenEntity = _reservationService.ShowReview(id);

                    if(ReviewForGivenEntity.Sucess == true)
                    {
                        return Ok(ReviewForGivenEntity.Review);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch(Exception _)
            {
                return Unauthorized();

            }

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]

        [HttpPost("employee/review")]
        public ActionResult<CreateReviewResponse> CreateReview(CreateReviewRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email);

            try
            {
                if (employee != null)
                {


                    var response = _reservationService.CreateReview(dto);
                    if (response.Success == true)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ADMIN,EMPLOYEE")]
        [HttpDelete("employee/reserve/{id}")]
        public ActionResult<CancelReservationResponse> CancelReservation(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            var jwt = authHeader.Substring(7);
            var JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            var email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            Employee employee = _employeeRepository.GetByEmail(email); ;

            try
            {
                if (employee != null)
                {
                   
                    var response = _reservationService.CancelReservation(id);
                    if (response.Success == true)
                    {
                        return Ok(new
                        {
                            message = "success"
                        });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

    }
}
