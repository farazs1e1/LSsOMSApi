﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OMSApi.Controllers
{
    [ApiController]
    [Authorize(Policy = AuthorizationPolicies.DirectUiInternal)]
    [Route("int/ord/api/locates/summary")]
    public class LocatesSummaryIntController : ControllerBase
    {
        private readonly ILocatesService locatesService;
        public LocatesSummaryIntController(ILocatesService locatesService)
        {
            this.locatesService = locatesService;
        }

        [HttpGet("subscribe")]
        public async Task<IActionResult> SubscribeAsync()
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocatesSummary>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("unsubscribe")]
        public async Task<IActionResult> UnsubscribeAsync()
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummary);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("available/subscribe/{account}/{symbol}")]
        public async Task<IActionResult> SubscribeAvailableAsync([Required(ErrorMessage = "Account not provided"), StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid Account entered")] string account, [Required(ErrorMessage = "Symbol not provided"), RegularExpression(Regexes.Symbol, ErrorMessage = "Invalid symbol entered")] string symbol)
        {
            var res = await locatesService.SubscribeAsync<ResultDataObject<SubscriptionLocatesSummary>>(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummaryWithSymbol, account, symbol);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }

        [HttpGet("available/unsubscribe")]
        public async Task<IActionResult> UnsubscribeAvailableAsync()
        {
            var res = await locatesService.UnsubscribeAsync(User.UserIdentifier(), User.OriginatingUserId(), User.ClientId(), QueryType.LocateSummaryWithSymbol);
            if (res == null)
                return BadRequest("Failure!");
            return Ok(res);
        }
    }
}
