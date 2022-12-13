using System;
using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionAuditTrails
    {
        public DateTime TransactTime { get; set; }
        public DateTime TradeDate { get; set; }
        public long MajVersionid { get; set; }
        public long VersionMaj { get; set; }
        public long VersionMin { get; set; }
        public double Quantity { get; set; }
        public double ExecutedQuantity { get; set; }
        public double LeavesQuantity { get; set; }
        public string OrderID { get; set; }
        public long QOrderID { get; set; }
        public double Price { get; set; }
        public string Symbol { get; set; }
        public string SideDesc { get; set; }
        public string Account { get; set; }
        public string ID { get; set; }
        public string Status { get; set; }
        public double Workable { get; set; }
        //public DateTime Time { get; set; }
        public string Time { get; set; }
        public string Text { get; set; }
        public double PercentExecuted { get; set; }
        public string Side { get; set; }

        [DataMember(Name = "CompleteQty")]
        public double OrderQty { get; set; }

        public string Destination { get; set; }
        public string DestinationDesc { get; set; }
        public string CommType { get; set; }
        public string OriginatingUserDesc { get; set; }
        public string OrdType { get; set; }
        public double StopPx { get; set; }
        public string TimeInForce { get; set; }
        public string Rule80A { get; set; }
        public string ClientID { get; set; }
        public string MsgType { get; set; }
        public long SeekInfo { get; set; }
        public string OatsInst { get; set; }
        public string OrdertypeInst { get; set; }
        public string HandlInst { get; set; }
        public bool IsAccepted { get; set; }
        public string ClOrdID { get; set; }
        public string SettlmntTyp { get; set; }
        public string ExecInst { get; set; }
        public double Commission { get; set; }
        public string TargetLocationID { get; set; }
        public string TIFDesc { get; set; }
        public string AccountTypeDesc { get; set; }
        public string SettlementTypeDesc { get; set; }
        public string CommisionTypeDesc { get; set; }
        public string OrderTypeDesc { get; set; }
        public string StatusDesc { get; set; }

        [DataMember(Name = "CompleteAvgPx")]
        public double AvgPx { get; set; }

        [DataMember(Name = "CompleteCumQty")]
        public double CumQty { get; set; }

        public double WorkableQty { get; set; }

        [DataMember(Name = "CompleteLeaveQty")]
        public double LeavesQty { get; set; }

        public string OrderRouteStatus { get; set; }
        public string BoothID { get; set; }
        public string PositionKey { get; set; }
        public string SymbolSfx { get; set; }
        public string SymbolWithoutSfx { get; set; }

        [DataMember(Name = "ComplianceId")]
        public string ComplianceID { get; set; }

        public string ExternalClordid { get; set; }
        public long ParentOrderid { get; set; }
        public string OrigClOrdID { get; set; }
        public string ExecRefID { get; set; }

        [DataMember(Name = "CompleteLastPx")]
        public double LastPx { get; set; }

        public string ExecID { get; set; }
        public string ExecType { get; set; }
        public string ExecTransType { get; set; }
        public string ExecTransTypeDesc { get; set; }

        [DataMember(Name = "CompleteLastShares")]
        public double LastShares { get; set; }

        public double ChangedShares { get; set; }
        public string ExternalExecid { get; set; }
        public string PropExecType { get; set; }
    }
}
