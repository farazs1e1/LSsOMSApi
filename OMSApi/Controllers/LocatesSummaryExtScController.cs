using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.ServerCommunicationExternal)]
    [Route("ext/ord/sc/api/locates/summary")]
    public class LocatesSummaryExtScController : ControllerBase
    {
        private readonly ILocatesService locatesService;
        public LocatesSummaryExtScController(ILocatesService locatesService)
        {
            this.locatesService = locatesService;
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync([Required] string userDesc)
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocatesSummary>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([Required] string userDesc)
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
