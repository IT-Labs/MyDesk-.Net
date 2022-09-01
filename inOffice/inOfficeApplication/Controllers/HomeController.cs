using FluentValidation.Results;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using inOfficeApplication.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace inOfficeApplication.Controllers
{
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
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CoworkerReservation([FromBody]ReservationRequest reservationRequest)
        {
            ReservationRequestValidation validationRules = new ReservationRequestValidation();
            ValidationResult validationResult = validationRules.Validate(reservationRequest);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _reservationService.CoworkerReserve(reservationRequest);
            return Ok($"Sucessfuly reserved desk for coworker with mail {reservationRequest.EmployeeEmail}");
        }

        [HttpGet("employee/all")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<EmployeeDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult AllEmployees()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<EmployeeDto> result = _employeeService.GetAll(take: take, skip: skip);

            return Ok(result);
        }

        [HttpGet("employee/future-reservation")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ReservationDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult EmployeeReservations()
        {
            List<ReservationDto> reservations = _reservationService.EmployeeReservations(GetEmployeeEmail());
            return Ok(reservations);
        }

        [HttpGet("employee/past-reservations")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ReservationDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult PastReservations()
        {
            List<ReservationDto> reservations = _reservationService.PastReservations(GetEmployeeEmail());
            return Ok(reservations);
        }

        [HttpGet("employee/review/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReviewDto))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ShowReview(int id)
        {
            ReviewDto review = _reviewService.ShowReview(id);
            return Ok(review);
        }

        [HttpPost("employee/review")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CreateReview([FromBody] ReviewDto reviewDto)
        {
            ReviewDtoValidation validationRules = new ReviewDtoValidation();
            ValidationResult validationResult = validationRules.Validate(reviewDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(x => x.ErrorMessage));
            }

            _reviewService.CreateReview(reviewDto);
            return Ok();
        }

        [HttpDelete("employee/reserve/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CancelReservation(int id)
        {
            _reservationService.CancelReservation(id);
            return Ok();
        }

        [HttpPut("admin/employee/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult SetAsAdmin(int id)
        {
            _employeeService.SetEmployeeAsAdmin(id);
            return Ok();
        }

        private string GetEmployeeEmail()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            string jwt = authHeader.Substring(7);
            JwtPayload jwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            return jwtSecurityTokenDecoded.Claims.First(x => x.Type == "preferred_username").Value;
        }
    }
}
