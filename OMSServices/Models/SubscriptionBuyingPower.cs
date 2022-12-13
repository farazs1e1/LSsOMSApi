namespace OMSServices.Models
{
    public class SubscriptionBuyingPower
    {
        public string Currency { get; set; }
        public string ID { get; set; }
        public string ClientID { get; set; }
        public double Cash { get; set; }
        public double Margin { get; set; }
        public string OriginatingUserDesc { get; set; }
        public string Firmcode { get; set; }
        public string CustomerCode { get; set; }
        // TODO get the formula reviewed.
        public double NetLimit { get; set; } // Formula to calculate BuyingPower = NetLimit - NetUsedLimit
        public double NetUsedLimit { get; set; }
    }
}
