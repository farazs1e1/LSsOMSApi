using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.ServerCommunicationExternal)]
    [Route("ext/ord/sc/api/orders")]
    public class OrdersExtScController : ControllerBase
    {
        private readonly IOrderManagementService orderManagementService;

        public OrdersExtScController(IOrderManagementService orderManagementService)
        {
            this.orderManagementService = orderManagementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([Required] string userDesc, OrderRequest orderRequest)
        {
            var result = await orderManagementService.CreateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), userDesc), User.UserIdentifier());
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
        public async Task<IActionResult> UpdateOrderAsync([Required] string userDesc, ModifyOrderRequest orderRequest)
        {
            var result = await orderManagementService.UpdateOrderAsync(orderRequest.ToBOEMsg(User.ClientId(), userDesc), User.UserIdentifier());
            if (string.IsNullOrWhiteSpace(result.ToString()))
                return Ok("Order successfully updated.");
            return BadRequest(result.ToString());
        }

        [HttpDelete("{qOrderID}/{userDesc}")]
        public IActionResult CancelOrderAsync(long qOrderID, string userDesc)
        {
            var result = orderManagementService.CancelOrderAsync(qOrderID, User.ClientId(), userDesc);
            return Ok(result);
        }
    }
}