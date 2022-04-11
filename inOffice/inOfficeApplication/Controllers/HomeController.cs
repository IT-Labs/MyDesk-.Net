using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IReservationService _reservationService;

        public HomeController(JwtService jwtService, IReservationService reservationService)
        {
            _jwtService = jwtService;
            _reservationService = reservationService;
        }

        [HttpPost("employee/reserve")]
        public ActionResult<ReservationResponse> Reservation(ReservationRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);
            
            try
            {                
                if (employee != null)
                {
                    
                    var response =_reservationService.Reserve(dto, employee);
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

        [HttpGet("employee/reserve")]
        public ActionResult<EmployeeReservationsResponse> EmployeeReservations()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);

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
        [HttpGet("employee/past-reservations")]
        public ActionResult<EmployeeReservationsResponse> PastReservations()
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);

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

        [HttpGet("employee/review/{id}")]
        public ActionResult<ReviewResponse> ShowReview(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);
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

        [HttpPost("employee/review")]
        public ActionResult<CreateReviewResponse> CreateReview(CreateReviewRequest dto)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);

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

        [HttpDelete("employee/reserve/{id}")]
        public ActionResult<CancelReservationResponse> CancelReservation(int id)
        {
            string authHeader = Request.Headers[HeaderNames.Authorization];

            Employee employee = _jwtService.EmployeeRoleVerification(authHeader);

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
