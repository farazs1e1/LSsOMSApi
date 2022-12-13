using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionPositionInt : SubscriptionPosition
    {
        [DataMember(Name = "ID")]
        public string Id { get; set; }
        public double CompleteDayBuyOrderQty { get; set; }
        public double CompleteDaySellLongOrderQty { get; set; }
        public double CompleteDaySellShortOrderQty { get; set; }
    }
}
