using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
using OMSApi.ResponseModels;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/subscription")]
    [Route("int/ord/api/v{version:apiVersion}/subscription")]
    [ApiVersion("1")]
    [ApiVersion("3")]
    public class SubscriptionIntController : ControllerBase
    {
        private readonly IOrderSubscriptionService orderSubscriptionService;
        public SubscriptionIntController(IOrderSubscriptionService orderSubscriptionService)
        {
            this.orderSubscriptionService = orderSubscriptionService;
        }

        [HttpGet("orders/subscribe")]
        public async Task<IActionResult> SubscribeOrdersAsync()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Orders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("orders/unsubscribe")]
        public async Task<IActionResult> UnsubscribeOrdersAsync()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Orders);
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
        public async Task<IActionResult> SubscribeOpenOrdersAsync()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OpenOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("openorders/unsubscribe")]
        public async Task<IActionResult> UnsubscribeOpenOrdersAsync()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OpenOrders);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("executions/subscribe")]
        public async Task<IActionResult> SubscribeExecutionsAsync()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionExecutionInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Executions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("executions/unsubscribe")]
        public async Task<IActionResult> UnsubscribeExecutionsAsync()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Executions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("positions/subscribe")]
        public async Task<IActionResult> SubscribePositionsAsync()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionPositionInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Positions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("positions/unsubscribe")]
        public async Task<IActionResult> UnsubscribePositionsAsync()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Positions);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpPost("orders/subscribe")]
        public async Task<IActionResult> SubscribeOrdersPostAsync(SubscriptionRequest request)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Orders, request.Accounts);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpPost("positions/subscribe")]
        public async Task<IActionResult> SubscribePositionsPostAsync(SubscriptionRequest request)
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionPositionInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Positions, request.Accounts);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        /// <summary>
        /// API version 3 actions:
        /// </summary>

        [HttpGet("orders/subscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<SubscriptionOrderInt>>> SubscribeOrdersV3Async()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Orders);
            if (res == null)
                return new Response<ResultDataObject<SubscriptionOrderInt>>(false, "Failure!");

            return new Response<ResultDataObject<SubscriptionOrderInt>>(res);
        }

        [HttpGet("orders/unsubscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<string>> UnsubscribeOrdersV3Async()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Orders);
            if (res == "Failure!")
                return new Response<string>(false, res);

            return new Response<string>(true, res);
        }

        [HttpGet("openorders/subscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<SubscriptionOrderInt>>> SubscribeOpenOrdersV3Async()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionOrderInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OpenOrders);
            if (res == null)
                return new Response<ResultDataObject<SubscriptionOrderInt>>(false, "Failure!");

            return new Response<ResultDataObject<SubscriptionOrderInt>>(res);
        }

        [HttpGet("openorders/unsubscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<string>> UnsubscribeOpenOrdersV3Async()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.OpenOrders);
            if (res == "Failure!")
                return new Response<string>(false, res);

            return new Response<string>(true, res);
        }

        [HttpGet("executions/subscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<SubscriptionExecutionInt>>> SubscribeExecutionsV3Async()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionExecutionInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Executions);
            if (res == null)
                return new Response<ResultDataObject<SubscriptionExecutionInt>>(false, "Failure!");

            return new Response<ResultDataObject<SubscriptionExecutionInt>>(res);
        }

        [HttpGet("executions/unsubscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<string>> UnsubscribeExecutionsV3Async()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Executions);
            if (res == "Failure!")
                return new Response<string>(false, res);

            return new Response<string>(true, res);
        }

        [HttpGet("positions/subscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<SubscriptionPositionInt>>> SubscribePositionsV3Async()
        {
            var res = await orderSubscriptionService.SubscribeAsync<ResultDataObject<SubscriptionPositionInt>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Positions);
            if (res == null)
                return new Response<ResultDataObject<SubscriptionPositionInt>>(false, "Failure!");

            return new Response<ResultDataObject<SubscriptionPositionInt>>(res);
        }

        [HttpGet("positions/unsubscribe")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<string>> UnsubscribePositionsV3Async()
        {
            var res = await orderSubscriptionService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.Positions);
            if (res == "Failure!")
                return new Response<string>(false, res);

            return new Response<string>(true, res);
        }
    }
}