using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.ServerCommunicationExternal)]
    [Route("ext/ord/sc/api/account/balances")]
    public class AccountBalancesExtScController : ControllerBase
    {
        private readonly IAccountBalancesService accountBalancesService;
        public AccountBalancesExtScController(IAccountBalancesService accountBalancesService)
        {
            this.accountBalancesService = accountBalancesService;
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync([Required] string userDesc)
        {
            var res = await accountBalancesService.SubscribeAsync<ResultDataObject<SubscriptionBuyingPower>>(User.UserIdentifier(), userDesc, User.ClientId());
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync([Required] string userDesc)
        {
            var res = await accountBalancesService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId());
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
