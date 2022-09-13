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
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("employee/reservations/all")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PaginationDto<ReservationDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ReservationsAll()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.AllReservations(take: take, skip: skip);

            return Ok(reservations);
        }

        [HttpGet("employee/future-reservation")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<ReservationDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult EmployeeReservations()
        {
            List<ReservationDto> reservations = _reservationService.FutureReservations(GetEmployeeEmail());
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

        [HttpPost("employee/reserve/coworker")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult CoworkerReservation([FromBody] ReservationRequest reservationRequest)
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

        private string GetEmployeeEmail()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];
            string jwt = authHeader.Substring(7);
            JwtPayload jwtSecurityTokenDecoded = new JwtSecurityToken(jwt).Payload;
            return jwtSecurityTokenDecoded.Claims.First(x => x.Type == "preferred_username").Value;
        }
    }
}
