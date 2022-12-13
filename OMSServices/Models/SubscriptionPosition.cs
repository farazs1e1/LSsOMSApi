using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionPosition
    {
        public string Symbol { get; set; }
        public string PositionString { get; set; }
        public double AvgPrice { get; set; }
        public double TotDollarOfTrade { get; set; }

        [DataMember(Name = "CompleteExecQty")]
        public double ExecQty { get; set; }

        public double RealizedPnL { get; set; }
        public string SymbolSfx { get; set; }
        public string OriginatingUserDesc { get; set; }
        public string Account { get; set; }
    }
}
