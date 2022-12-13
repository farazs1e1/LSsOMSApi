using System;

namespace OMSServices.Models
{
    public class SubscriptionLocatesSummary
    {
        public string ID { get; set; }
        public string Symbol { get; set; }
        public string Account { get; set; }
        public string SymbolSfx { get; set; }
        public DateTime TransactTime { get; set; }
        public string ClientID { get; set; }
        public string LocateType { get; set; }
        public string BoothID { get; set; }
        public string OriginatingUserDesc { get; set; }
        public double EtbQty { get; set; }
    }
}