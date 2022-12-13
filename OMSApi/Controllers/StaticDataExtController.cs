using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;
using OMSServices.Models;
using OMSServices.Enum;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiExternal)]
    [Route("ext/ord/api/staticData")]
    public class StaticDataExtController : ControllerBase
    {
        private readonly IStaticDataService staticDataService;

        public StaticDataExtController(IStaticDataService staticDataService)
        {
            this.staticDataService = staticDataService;
        }

        [HttpGet("Destination")]
        public async Task<IActionResult> GetDestinationAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Destination, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("Account")]
        public async Task<IActionResult> GetAccountAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Account, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("Side")]
        public async Task<IActionResult> GetSideAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Side, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("TIF")]
        public async Task<IActionResult> GetTIFAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TIF, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("OrdType")]
        public async Task<IActionResult> GetOrdTypeAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.OrdType, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("LocateTIF")]
        public async Task<IActionResult> GetLocateTIFAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.LocateTIF, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }
    }
}
