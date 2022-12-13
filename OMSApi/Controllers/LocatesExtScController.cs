using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
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
    [Route("ext/ord/sc/api/locates")]
    public class LocatesExtScController : ControllerBase
    {
        private readonly ILocatesService locatesService;
        public LocatesExtScController(ILocatesService locatesService)
        {
            this.locatesService = locatesService;
        }

        [HttpPost]
        public async Task<IActionResult> LocateRequest([Required] string userDesc, LocateRequest locateRequest)
        {
            var result = await locatesService.LocateRequest(locateRequest.ToBOEMsg(User.ClientId(), userDesc), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync([Required] string userDesc)
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocates>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Locates);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([Required] string userDesc)
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Locates);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
