using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/audittrails")]
    public class AuditTrailsIntController : ControllerBase
    {
        private readonly IAuditTrailsService auditTrailsService;
        public AuditTrailsIntController(IAuditTrailsService auditTrailsService)
        {
            this.auditTrailsService = auditTrailsService;
        }

        [HttpGet("subscribe/{qOrderID}")]
        public async Task<IActionResult> SubscribeOrdersAsync(long qOrderID)
        {
            var res = await auditTrailsService.SubscribeAsync<ResultDataObject<SubscriptionAuditTrails>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), qOrderID);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe/{qOrderID}")]
        public async Task<IActionResult> UnsubscribeOrdersAsync(long qOrderID)
        {
            var res = await auditTrailsService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), qOrderID);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
