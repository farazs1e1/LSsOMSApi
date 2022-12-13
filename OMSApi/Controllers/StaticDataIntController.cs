using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OMSServices.Services;
using OMSServices.Utils;
using System.Threading.Tasks;
using OMSServices.Models;
using OMSServices.Enum;
using OMSApi.ResponseModels;
using System.Dynamic;
using System.ComponentModel.DataAnnotations;
using OMSApi.Attributes;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/staticData")]
    [Route("int/ord/api/v{version:apiVersion}/staticData")]
    [ApiVersion("1")]
    [ApiVersion("3")]
    public class StaticDataIntController : ControllerBase
    {
        private readonly IStaticDataService staticDataService;

        public StaticDataIntController(IStaticDataService staticDataService)
        {
            this.staticDataService = staticDataService;
        }

        [HttpGet("Side")]
        public async Task<IActionResult> GetSideAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Side, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
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

        [HttpGet("TIF")]
        public async Task<IActionResult> GetTIFAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TIF, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("CommType")]
        public async Task<IActionResult> GetCommTypeAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.CommType, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

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

        [HttpGet("TimeZone")]
        public async Task<IActionResult> GetTimeZoneAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TimeZone, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

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

        [HttpGet("MktTopPerfCateg")]
        public async Task<IActionResult> GetMktTopPerfCategAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfCateg, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        [HttpGet("MktTopPerfExchange")]
        public async Task<IActionResult> GetMktTopPerfExchangeAsync()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfExchange, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return BadRequest("Failure!");

            return Ok(result);
        }

        /// <summary>
        /// API version 3 actions:
        /// </summary>

        [HttpGet("Side")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetSideV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Side, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("Destination")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetDestinationV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Destination, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("Account")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetAccountV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Account, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("TIF")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetTIFV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TIF, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("CommType")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetCommTypeV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.CommType, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("OrdType")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetOrdTypeV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.OrdType, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("TimeZone")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetTimeZoneV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.TimeZone, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("LocateTIF")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetLocateTIFV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.LocateTIF, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("MktTopPerfCateg")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetMktTopPerfCategV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfCateg, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("MktTopPerfExchange")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<StaticDataValues>>> GetMktTopPerfExchangeV3Async()
        {
            var result = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.MktTopPerfExchange, User.OriginatingUserId(), User.ClientId(), User.UserIdentifier());

            if (result == null)
                return new Response<ResultDataObject<StaticDataValues>>(false, "Failure!", null);

            return new Response<ResultDataObject<StaticDataValues>>(result);
        }

        [HttpGet("ETBHTB/{account}/{symbol}")]
        [MapToApiVersion("3")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<Response<ResultDataObject<ExpandoObject>>> GetEtbHtbV3Async([Required(ErrorMessage = "Account not provided"), StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid Account entered")] string account, [Required(ErrorMessage = "Symbol not provided"), RegularExpression(Regexes.Symbol, ErrorMessage = "Invalid symbol entered")] string symbol)
        {
            var result = await staticDataService.GetStaticDataEasyToBorrowAsync<ExpandoObject>(User.OriginatingUserId(), account, symbol);

            if (result == null)
            {
                return new Response<ResultDataObject<ExpandoObject>>(false, "Failure!", null);
            }
            return new Response<ResultDataObject<ExpandoObject>>(result);
        }
    }
}
