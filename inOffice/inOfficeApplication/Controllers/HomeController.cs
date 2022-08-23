using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Entities;
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

        [HttpPost("employee/reserve/coworker")]
        public ActionResult<ReservationResponse> CoworkerReservation(CoworkerReservationRequest dto)
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

        [HttpGet("employee/all")]
        public ActionResult<List<CustomEmployee>> AllEmployees()
        {
            List<CustomEmployee> result = _employeeService.GetAll();

            return Ok(result);
        }

        [HttpGet("employee/future-reservation")]
        public ActionResult<EmployeeReservationsResponse> EmployeeReservations()
        {
            Employee employee = GetEmployee();

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

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

        [HttpGet("employee/past-reservations")]
        public ActionResult<EmployeeReservationsResponse> PastReservations()
        {
            Employee employee = GetEmployee();

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

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

        [HttpGet("employee/review/{id}")]
        public ActionResult<ReviewResponse> ShowReview(int id)
        {
            Employee employee = GetEmployee();

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

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

        [HttpPost("employee/review")]
        public ActionResult<CreateReviewResponse> CreateReview(CreateReviewRequest dto)
        {
            Employee employee = GetEmployee();

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

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

        [HttpDelete("employee/reserve/{id}")]
        public ActionResult<CancelReservationResponse> CancelReservation(int id)
        {
            Employee employee = GetEmployee();

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

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

        private Employee GetEmployee()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            string jwt = authHeader.Substring(7);
            JwtPayload jwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            string email = jwtSecurityTokenDecoded.Claims.First(x => x.Type == "preferred_username").Value;
            return _employeeService.GetByEmail(email);
        }
    }
}
