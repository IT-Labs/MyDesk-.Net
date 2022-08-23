using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Requests;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.DTO;
using inOfficeApplication.Data.Entities;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Mvc;

namespace inOfficeApplication.Controllers
{
    [Route("")]
    [ApiController]
    public class OfficeController : Controller
    {
        private readonly IOfficeService _officeService;

        public OfficeController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        [HttpPost("admin/office")]
        public ActionResult<OfficeResponse> AddNewOffice(NewOfficeRequest dto)
        {
            OfficeResponse response = _officeService.CreateNewOffice(dto);
            if (response.Success != true)
            {
                return Conflict("There is allready office with the same name");
            }
            else
            {
                return Created("Success", response);
            }
        }

        [HttpGet("admin/office/image/{id}")]
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

        [HttpPut("admin/office/{id}")]
        public ActionResult<OfficeResponse> Edit(int id, OfficeRequest dto)
        {
            dto.Id = id;
            OfficeResponse result = _officeService.UpdateOffice(dto);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("admin/office/{id}")]
        public ActionResult<OfficeResponse> Delete(int id)
        {
            OfficeResponse result = _officeService.DeleteOffice(id);
            if (result.Success)
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

        [HttpGet("admin/offices")]
        public ActionResult<IEnumerable<OfficeDto>> GetAllOffices()
        {
            Utilities.GetPaginationParameters(Request, out int? take, out int? skip);
            OfficeListResponse offices = _officeService.GetAllOffices(take: take, skip: skip);

            if (offices.Success)
            {
                return Ok(offices.Offices);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
