using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using FluentValidation.Results;
using MyDesk.Data.Interfaces.BusinessLogic;
using MyDesk.Data.Requests;
using MyDesk.Data.DTO;
using MyDesk.Data.Utils;
using MyDesk.Application.Validations;

namespace MyDesk.Application.Controllers
{
    [ApiController]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly string[] identityClaims = new string[] { "preferred_username", "email" };

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("employee/future-reservation/all")]
        public IActionResult GetAllFutureReservations()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.FutureReservations(string.Empty, take: take, skip: skip);

            return Ok(reservations);
        }

        [HttpGet("employee/past-reservations/all")]
        public IActionResult GetAllPastReservations()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.PastReservations(string.Empty, take: take, skip: skip);

            return Ok(reservations);
        }

        [HttpGet("employee/future-reservation")]
        public IActionResult EmployeeReservations()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.FutureReservations(GetEmployeeEmail(), take: take, skip: skip);
            return Ok(reservations);
        }

        [HttpGet("employee/past-reservations")]
        public IActionResult PastReservations()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            PaginationDto<ReservationDto> reservations = _reservationService.PastReservations(GetEmployeeEmail(), take: take, skip: skip);
            return Ok(reservations);
        }

        [HttpPost("employee/reserve/coworker")]
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
            return jwtSecurityTokenDecoded.Claims.First(x => identityClaims.Contains(x.Type)).Value;
        }
    }
}
