using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
using OMSApi.ResponseModels;
using OMSServices.Data;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/orders")]
    [Route("int/ord/api/v{version:apiVersion}/orders")]
    [ApiVersion("1")]
    [ApiVersion("2")]
    [ApiVersion("3")]
    public class OrdersIntController : ControllerBase
    {
        private readonly IOrderManagementService orderManagementService;

        public OrdersIntController(IOrderManagementService orderManagementService)
        {
            this.orderManagementService = orderManagementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(OrderRequestInt orderRequest)
        {
            var result = await orderManagementService.CreateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            if (result.Success)
            {
                return Ok(result.Message);
            }
            else if (result.Confirmation)
            {
                return Accepted(result.ServerResponse);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }

        [HttpPost]
        [MapToApiVersion("2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CreateOrderV2Async(OrderRequestInt orderRequest)
        {
            var result = await orderManagementService.CreateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            if (result.Success)
                return Ok(new Response<string>(true, result.Message));
            else if (result.Confirmation)
                return Accepted(result.ServerResponse);

            return BadRequest(new Response<string>(false, result.Message));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrderAsync(ModifyOrderRequest orderRequest)
        {
            var result = await orderManagementService.UpdateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            if (string.IsNullOrWhiteSpace(result.ToString()))
                return Ok("Order successfully updated.");
            return BadRequest(result.ToString());
        }

        [HttpPut]
        [MapToApiVersion("2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UpdateOrderV2Async(ModifyOrderRequest orderRequest)
        {
            var result = (await orderManagementService.UpdateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier())) as string;
            if (string.IsNullOrWhiteSpace(result))
                return Ok(new Response<string>(true, "Order successfully updated."));

            return BadRequest(new Response<string>(false, result));
        }

        [HttpDelete("{qOrderID}")]
        public object CancelOrderAsync(long qOrderID)
        {
            object result = orderManagementService.CancelOrderAsync(qOrderID, User.ClientId(), User.OriginatingUserId());
            return result;
        }

        [HttpDelete("{qOrderID}")]
        [MapToApiVersion("2")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Response<string> CancelOrderAsyncV2(long qOrderID)
        {
            var result = orderManagementService.CancelOrderV3(qOrderID, User.ClientId(), User.OriginatingUserId());
            return new Response<string>(result.Status, result.Message);
        }


        /// <summary>
        /// API version 3 actions:
        /// </summary>

        [HttpPost]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ServerResponse>> CreateOrderV3Async(OrderRequestInt orderRequest)
        {
            var result = await orderManagementService.CreateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            return new Response<ServerResponse>(result.Success, result.Message, result.ServerResponse);
        }

        [HttpPut]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<string>> UpdateOrderV3Async(ModifyOrderRequest orderRequest)
        {
            var result = (await orderManagementService.UpdateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier())) as string;
            if (string.IsNullOrWhiteSpace(result))
            {
                return new Response<string>(true, "Order successfully updated.");
            }
            return new Response<string>(false, result);
        }

        [HttpDelete("{qOrderID}")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Response<string> CancelOrderAsyncV3(long qOrderID)
        {
            var result = orderManagementService.CancelOrderV3(qOrderID, User.ClientId(), User.OriginatingUserId());
            return new Response<string>(result.Status, result.Message);
        }
    }
}