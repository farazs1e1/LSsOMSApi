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
    [Authorize(Policy = AuthorizationPolicies.ServerCommunicationInternal)]
    [Route("int/ord/sc/api/subscription")]
    public class SubscriptionIntScController : ControllerBase
    {
        private readonly IOrderSubscriptionService orderSubscriptionService;
        public SubscriptionIntScController(IOrderSubscriptionService orderSubscriptionService)
        {
            this.orderSubscriptionService = orderSubscriptionService;
        }

        [HttpGet("orders/subscribe")]
        public async Task<IActionResult> SubscribeOrdersAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Orders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("orders/unsubscribe")]
        public async Task<IActionResult> UnsubscribeOrdersAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Orders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("optionorders/subscribe")]
        public async Task<IActionResult> SubscribeOptionOrdersAsync()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OptionOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("optionorders/unsubscribe")]
        public async Task<IActionResult> UnsubscribeOptionOrdersAsync()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OptionOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("openorders/subscribe")]
        public async Task<IActionResult> SubscribeOpenOrdersAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.OpenOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("openorders/unsubscribe")]
        public async Task<IActionResult> UnsubscribeOpenOrdersAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.OpenOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("executions/subscribe")]
        public async Task<IActionResult> SubscribeExecutionsAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionExecutionInt>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Executions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("executions/unsubscribe")]
        public async Task<IActionResult> UnsubscribeExecutionsAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Executions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("positions/subscribe")]
        public async Task<IActionResult> SubscribePositionsAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionPositionInt>>(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Positions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("positions/unsubscribe")]
        public async Task<IActionResult> UnsubscribePositionsAsync([Required] string userDesc)
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), userDesc, User.ClientId(), QueryType.Positions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
