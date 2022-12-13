using System;
using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionExecution
    {
        public long QOrderID { get; set; }
        public double Price { get; set; }
        public string Symbol { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public DateTime TransactTime { get; set; }
        public string OrdStatus { get; set; }
        public string OriginatingUserDesc { get; set; }
        public string ExecBroker { get; set; }
        public string TimeInForce { get; set; }
        public string OrdType { get; set; }

        [DataMember(Name = "CompleteQty")]
        public double OrderQty { get; set; }

        public string OrigClOrdID { get; set; }
        public string ExecRefID { get; set; }

        [DataMember(Name = "CompleteLastPx")]
        public double LastPx { get; set; }

        public string ExecID { get; set; }
        public string ExecType { get; set; }
        public string ExecTransType { get; set; }
        public string ExecTransTypeDesc { get; set; }

        [DataMember(Name = "CompleteLeaveQty")]
        public double LeavesQty { get; set; }

        [DataMember(Name = "CompleteLastShares")]
        public double LastShares { get; set; }

        [DataMember(Name = "CompleteCumQty")]
        public double CumQty { get; set; }

        [DataMember(Name = "CompleteAvgPx")]
        public double AvgPx { get; set; }

        public DateTime TradeDate { get; set; }
        public string SymbolSfx { get; set; }
        public string SideDesc { get; set; }
        public string Destination { get; set; }
        public string Account { get; set; }
    }
}
