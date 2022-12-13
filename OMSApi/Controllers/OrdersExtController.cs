using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiExternal)]
    [Route("ext/ord/api/orders")]
    public class OrdersExtController : ControllerBase
    {
        private readonly IOrderManagementService orderManagementService;

        public OrdersExtController(IOrderManagementService orderManagementService)
        {
            this.orderManagementService = orderManagementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(OrderRequest orderRequest)
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

        [HttpPut]
        public async Task<IActionResult> UpdateOrderAsync(ModifyOrderRequest orderRequest)
        {
            var result = await orderManagementService.UpdateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), User.OriginatingUserId()), User.UserIdentifier());
            if (string.IsNullOrWhiteSpace(result.ToString()))
                return Ok("Order successfully updated.");
            return BadRequest(result.ToString());
        }

        [HttpDelete("{qOrderID}")]
        public IActionResult CancelOrderAsync(long qOrderID)
        {
            var result = orderManagementService.CancelOrderAsync(qOrderID, User.ClientId(), User.OriginatingUserId());
            return Ok(result);
        }
    }
}