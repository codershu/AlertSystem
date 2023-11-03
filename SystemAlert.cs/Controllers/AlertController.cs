using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SystemAlert.API.Controllers
{
    [Route("api/alerts")]
    [ApiController]
    [Authorize]
    public class AlertController : Controller
    {
        private readonly IAlertHandler _alertService;

        public AlertController(IAlertHandler alertService)
        {
            _alertService = alertService;
        }

        [HttpGet("types")]
        [Mcit_Authrorize(ApiPermissions.Alert.View)]
        public async Task<ActionResult<Response<List<AlertTypeItem>>>> GetAlertTypes([FromHeader] Guid customerId)
        {
            return await _alertService.GetAlertTypes();
        }

        [HttpGet("configurations")]
        [Mcit_Authrorize(ApiPermissions.Alert.View)]
        public async Task<ActionResult<Response<List<AlertConfigurationItem>>>> GetAlertConfigurations([FromHeader] Guid customerId)
        {
            return await _alertService.GetAlertConfigurations(customerId);
        }

        [HttpPost("configurations")]
        [Mcit_Authrorize(ApiPermissions.Alert.Create)]
        public async Task<ActionResult<Response<bool>>> CreateAlertConfiguration([FromHeader] Guid customerId, AlertConfigurationItem request)
        {
            return await _alertService.CreateAlertConfiguration(customerId, request);
        }

        [HttpPut("configurations")]
        [Mcit_Authrorize(ApiPermissions.Alert.Edit)]
        public async Task<ActionResult<Response<bool>>> UpdateAlertConfiguration([FromHeader] Guid customerId, AlertConfigurationItem request)
        {
            return await _alertService.UpdateAlertConfiguration(customerId, request);
        }

        [HttpDelete("configurations/{alertConfigurationId}")]
        [Mcit_Authrorize(ApiPermissions.Alert.Delete)]
        public async Task<ActionResult<Response<bool>>> DeleteAlertConfiguration([FromHeader] Guid customerId, Guid alertConfigurationId)
        {
            return await _alertService.DeleteAlertConfiguration(alertConfigurationId);
        }
    }
}
