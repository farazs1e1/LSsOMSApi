using OMSApi.Attributes;
using OMSServices.Enum;
using System.Collections.Generic;

namespace OMSApi.Models
{
    public class SubscriptionRequest
    {
        [StaticDataListValidation(QueryType.Account)]
        public List<string> Accounts { get; set; }
    }
}
