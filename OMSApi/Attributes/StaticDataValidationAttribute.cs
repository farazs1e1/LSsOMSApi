using Microsoft.AspNetCore.Http;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OMSApi.Attributes
{
    public class StaticDataValidationAttribute : ValidationAttribute
    {
        private readonly QueryType QueryType;

        public StaticDataValidationAttribute(QueryType queryType)
        {
            QueryType = queryType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            // resolve dependencies
            var staticDataService = (IStaticDataService)validationContext.GetService(typeof(IStaticDataService));
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            // expected values
            var userDesc = httpContextAccessor.HttpContext.User.OriginatingUserId();
            var clientId = httpContextAccessor.HttpContext.User.ClientId();
            var userIdentifier = httpContextAccessor.HttpContext.User.UserIdentifier();
            if (string.IsNullOrWhiteSpace(userDesc))
            {
                userDesc = httpContextAccessor.HttpContext.Request.Query["userDesc"].FirstOrDefault();
            }
            var expectedValues = ExpectedValues(staticDataService, userDesc, clientId, userIdentifier);

            // if expected values is null
            if (expectedValues == null)
                return new ValidationResult(ErrorMessage, new string[] { validationContext.MemberName });

            if (expectedValues.Any(x => x.Equals((string)value)))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage, new string[] { validationContext.MemberName });
        }

        private List<string> ExpectedValues(IStaticDataService staticDataService, string userDesc, string clientId, string userIdentifier)
        {
            var expectedValues = staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType, userDesc, clientId, userIdentifier).Result;
            return expectedValues.EventData.Select(x => x.Value).ToList();
        }
    }
}
