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
    [Route("int/ord/api/account/balances")]
    public class AccountBalancesIntController : ControllerBase
    {
        private readonly IAccountBalancesService accountBalancesService;
        public AccountBalancesIntController(IAccountBalancesService accountBalancesService)
        {
            this.accountBalancesService = accountBalancesService;
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync()
        {
            var res = await accountBalancesService.SubscribeAsync<ResultDataObject<SubscriptionBuyingPower>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId());
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync()
        {
            var res = await accountBalancesService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId());
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
