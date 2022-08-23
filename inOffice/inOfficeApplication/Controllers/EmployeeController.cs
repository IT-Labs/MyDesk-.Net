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
        public ActionResult<IEnumerable<OfficeDto>> GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            OfficeListResponse offices = _officeService.GetAllOffices(take: take, skip: skip);

            if (offices.Success == true)
            {
                return Ok(offices.Offices);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("employee/office/image/{id}")]
        public ActionResult<OfficeResponse> ImageUrl(int id)
        {
            Office office = _officeService.GetDetailsForOffice(id);
            if (office.OfficeImage != null)
            {
                return Ok(office.OfficeImage);
            }
            else
            {
                return BadRequest("Image not found");
            }
        }
    }
}
