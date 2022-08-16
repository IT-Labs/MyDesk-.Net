﻿using inOffice.BusinessLogicLayer;
using inOffice.BusinessLogicLayer.Interface;
using inOffice.BusinessLogicLayer.Responses;
using inOfficeApplication.Data.Models;
using inOfficeApplication.Data.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
        [HttpGet("employee/offices")]
        public ActionResult<IEnumerable<Office>> GetAllOffices()
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Constants.AllRoles)]
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
