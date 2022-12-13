using System;
using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionOrder
    {
        public long QOrderID { get; set; }
        public double Price { get; set; }
        public string Symbol { get; set; }
        public string SideDesc { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }
        public string Side { get; set; }

        [DataMember(Name = "ComplianceId")]
        public string ComplianceID { get; set; }

        [DataMember(Name = "CompleteQty")]
        public double OrderQty { get; set; }

        public string OriginatingUserDesc { get; set; }
        public string OrdType { get; set; }
        public double StopPx { get; set; }
        public string TimeInForce { get; set; }
        public DateTime TransactTime { get; set; }
        public string ClOrdID { get; set; }

        [DataMember(Name = "TIFDesc")]
        public string TifDesc { get; set; }

        public string OrderTypeDesc { get; set; }
        public string StatusDesc { get; set; }

        [DataMember(Name = "CompleteAvgPx")]
        public double AvgPx { get; set; }

        [DataMember(Name = "CompleteCumQty")]
        public double CumQty { get; set; }

        public double WorkableQty { get; set; }

        [DataMember(Name = "CompleteLeaveQty")]
        public double LeavesQty { get; set; }

        public string SymbolSfx { get; set; }
        public string SymbolWithoutSfx { get; set; }

        [DataMember(Name = "Groupid")] // TODO review it when this property is added in the QueryServer.
        public string LocateID { get; set; }

        [DataMember(Name = "Contactname")] // TODO review it when this property is added in the QueryServer.
        public string ContactName { get; set; }

        [DataMember(Name = "LocateReqd")] // TODO review it when this property is added in the QueryServer.
        public bool LocateRequired { get; set; }

        public double LocateRate { get; set; }
        public string Destination { get; set; }
        public string Account { get; set; }
    }
}
