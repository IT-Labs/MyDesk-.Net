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
        private readonly IEmployeeService _employeeService;
        private readonly IApplicationParmeters applicationParmeters;
        public EmployeeController(IEmployeeService employeeService, IApplicationParmeters applicationParmeters)
        {
            _employeeService = employeeService;
            this.applicationParmeters = applicationParmeters;
        }

        [HttpGet("params/all")]
        public IActionResult Test()
        {
            string issuer = applicationParmeters.GetJwtIssuer();
            string useCustomToken = applicationParmeters.GetSettingsUseCustomBearerToken();
            string signingKey = applicationParmeters.GetSettingsCustomBearerTokenSigningKey();
            string sentimentAPI = applicationParmeters.GetSettingsSentimentEndpoint();

            return Ok($"Issuer: {issuer}, UseCustomBearerToken: {useCustomToken}, CustomBearerTokenSigningKey: {signingKey}, SentimentEndpoint: {sentimentAPI}");
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
    }
}
