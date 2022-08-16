using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
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
        private readonly IEmployeeService _employeeService;
        private readonly IReviewService _reviewService;

        public HomeController(IReservationService reservationService,
            IEmployeeService employeeRepository,
            IReviewService reviewService)
        {
            _reservationService = reservationService;
            _employeeService = employeeRepository;
            _reviewService = reviewService;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpPost("employee/reserve")]
        public ActionResult<ReservationResponse> Reservation(ReservationRequest dto)
        {
            try
            {
                Employee employee = GetEmployee();

                ReservationResponse response = _reservationService.Reserve(dto, employee);
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpPost("employee/reserve/coworker")]
        public ActionResult<ReservationResponse> CoworkerReservation(CoworkerReservationRequest dto)
        {
            try
            {
                ReservationResponse response = _reservationService.CoworkerReserve(dto);
                if (response.Success == true)
                {
                    return Ok($"Sucessfuly reserved desk for coworker with mail {dto.CoworkerMail}");
                }
                else
                {
                    return Conflict($"Reservation for that time period allready exists for {dto.CoworkerMail}");
                }
            }
            catch (Exception _)
            {
                return BadRequest();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/all")]
        public ActionResult<List<CustomEmployee>> AllEmployees()
        {
            try
            {
                List<Employee> employees = _employeeService.GetAll();
                List<CustomEmployee> result = new List<CustomEmployee>();

                foreach (Employee employee in employees)
                {
                    result.Add(new CustomEmployee(employee.Id, employee.FirstName, employee.LastName, employee.Email, employee.JobTitle));
                }

                IEnumerable<CustomEmployee> filtereResult = result.DistinctBy(x => x.Email);

                return Ok(filtereResult);
            }
            catch (Exception _)
            {
                return NotFound();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/future-reservation")]
        public ActionResult<EmployeeReservationsResponse> EmployeeReservations()
        {
            try
            {
                Employee employee = GetEmployee();

                if (employee != null)
                {
                    EmployeeReservationsResponse response = _reservationService.EmployeeReservations(employee);

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/past-reservations")]
        public ActionResult<EmployeeReservationsResponse> PastReservations()
        {
            try
            {
                Employee employee = GetEmployee();

                if (employee != null)
                {
                    EmployeeReservationsResponse response = _reservationService.PastReservations(employee);

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/review/{id}")]
        public ActionResult<ReviewResponse> ShowReview(int id)
        {
            try
            {
                Employee employee = GetEmployee();

                if (employee != null)
                {
                    ReviewResponse ReviewForGivenEntity = _reviewService.ShowReview(id);

                    if (ReviewForGivenEntity.Sucess == true)
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
            catch (Exception _)
            {
                return Unauthorized();
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpPost("employee/review")]
        public ActionResult<CreateReviewResponse> CreateReview(CreateReviewRequest dto)
        {
            try
            {
                Employee employee = GetEmployee();

                if (employee != null)
                {
                    CreateReviewResponse response = _reviewService.CreateReview(dto);

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpDelete("employee/reserve/{id}")]
        public ActionResult<CancelReservationResponse> CancelReservation(int id)
        {
            try
            {
                Employee employee = GetEmployee();

                if (employee != null)
                {
                    CancelReservationResponse response = _reservationService.CancelReservation(id);
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

        private Employee GetEmployee()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            string jwt = authHeader.Substring(7);
            JwtPayload JwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            string email = JwtSecurityTokenDecoded.ElementAt(9).Value.ToString();
            return _employeeService.GetByEmail(email);
        }
    }
}
