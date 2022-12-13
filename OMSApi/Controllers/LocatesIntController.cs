using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/locates")]
    public class LocatesIntController : ControllerBase
    {
        private readonly ILocatesService locatesService;
        public LocatesIntController(ILocatesService locatesService)
        {
            this.locatesService = locatesService;
        }

        [HttpPost]
        public async Task<IActionResult> LocateRequest(LocateRequest locateRequest)
        {     
            var result = await locatesService.LocateRequest(locateRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            return Ok(result);
        }

        [HttpPost("acquire")]
        public async Task<IActionResult> LocateAcquire(LocateAcquireRequest locateAcquire)
        {
            var result = await locatesService.LocateAcquire(locateAcquire.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync()
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocates>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Locates);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync()
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Locates);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
