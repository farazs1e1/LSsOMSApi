using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiExternal)]
    [Route("ext/ord/api/locates/summary")]
    public class LocatesSummaryExtController : ControllerBase
    {
        private readonly ILocatesService locatesService;
        public LocatesSummaryExtController(ILocatesService locatesService)
        {
            this.locatesService = locatesService;
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync()
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocatesSummary>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync()
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
