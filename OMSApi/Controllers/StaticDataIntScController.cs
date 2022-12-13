using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OMSServices.Services;
using OMSServices.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using OMSServices.Models;
using OMSServices.Enum;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.ServerCommunicationInternal)]
    [Route("int/ord/sc/api/staticData")]
    public class StaticDataIntScController : ControllerBase
    {
        private readonly IStaticDataService staticDataService;

        public StaticDataIntScController(IStaticDataService staticDataService)
        {
            this.staticDataService = staticDataService;
        }


        [HttpGet("Side")]
        public async Task<IActionResult> GetSideAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Side, userDesc, User.ClientId(), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("Destination")]
        public async Task<IActionResult> GetDestinationAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Destination, userDesc, User.ClientId(), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("Account")]
        public async Task<IActionResult> GetAccountAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Account, userDesc, User.ClientId(), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("TIF")]
        public async Task<IActionResult> GetTIFAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TIF, userDesc, User.ClientId(), User.UserIdentifier());
            return Ok(result);
        }

        [HttpGet("OrdType")]
        public async Task<IActionResult> GetOrdTypeAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.OrdType, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("TimeZone")]
        public async Task<IActionResult> GetTimeZoneAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TimeZone, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("CommType")]
        public async Task<IActionResult> GetCommTypeAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.CommType, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("LocateTIF")]
        public async Task<IActionResult> GetLocateTIFAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.LocateTIF, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("MktTopPerfCateg")]
        public async Task<IActionResult> GetMktTopPerfCategAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfCateg, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("MktTopPerfExchange")]
        public async Task<IActionResult> GetMktTopPerfExchangeAsync([Required] string userDesc)
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfExchange, userDesc, User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }
    }
}
