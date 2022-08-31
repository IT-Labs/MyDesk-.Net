using inOffice.BusinessLogicLayer.Interface;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace inOfficeApplication.Controllers
{
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public EmployeeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet("employee/offices")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<OfficeDto>))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("employee/office/image/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public IActionResult ImageUrl(int id)
        {
            OfficeDto office = _officeService.GetDetailsForOffice(id);
            return Ok(office.OfficeImage);
        }
    }
}
