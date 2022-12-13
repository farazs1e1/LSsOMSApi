using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OMSApi.Attributes
{
    public class StaticDataListValidationAttribute : ValidationAttribute
    {
        private readonly QueryType _queryType;

        public StaticDataListValidationAttribute(QueryType queryType)
        {
            _queryType = queryType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is not List<string> staticDataValues)
            {
                throw new System.InvalidCastException($"{validationContext.MemberName} is expected to be a list of string values of {_queryType}.");
            }

            // return early if the incoming list is empty.
            if (!staticDataValues.Any()) return ValidationResult.Success;

            // resolve dependencies
            var staticDataService = (IStaticDataService)validationContext.GetRequiredService(typeof(IStaticDataService));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetRequiredService(typeof(IHttpContextAccessor));

            var clientId = httpContextAccessor.HttpContext.User.ClientId();
            var userIdentifier = httpContextAccessor.HttpContext.User.UserIdentifier();
            var userDesc = httpContextAccessor.HttpContext.User.OriginatingUserId();
            if (string.IsNullOrWhiteSpace(userDesc))
            {
                userDesc = httpContextAccessor.HttpContext.Request.Query["userDesc"].FirstOrDefault();
            }
            var expectedStaticDataValues = ExpectedValues(staticDataService, userDesc, clientId, userIdentifier);

            var invalidValues = staticDataValues.Except(expectedStaticDataValues, StringComparer.Ordinal).ToList();
            if (invalidValues.Any())
            {
                return new ValidationResult($"Invalid static data values: {string.Join(", ", invalidValues)}", new string[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }

        private List<string> ExpectedValues(IStaticDataService staticDataService, string userDesc, string clientId, string userIdentifier)
        {
            var expectedValues = staticDataService.GetStaticDataAsync<StaticDataValues>(_queryType, userDesc, clientId, userIdentifier).Result;
            return expectedValues.EventData.ConvertAll(x => x.Value);
        }
    }
}
