using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public EmployeeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpGet("employee/offices")]
        public IActionResult GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            List<OfficeDto> offices = _officeService.GetAllOffices(take: take, skip: skip);

            return Ok(offices);
        }

        [HttpGet("employee/office/image/{id}")]
        public IActionResult ImageUrl(int id)
        {
            OfficeDto office = _officeService.GetDetailsForOffice(id);
            return Ok(office.OfficeImage);
        }
    }
}
