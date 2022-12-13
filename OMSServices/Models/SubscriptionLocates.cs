using System;
using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionLocates
    {
        public string OrdType { get; set; }
        public string QuoteReqID { get; set; }
        public string OrdStatus { get; set; }
        public double OrderQty { get; set; }
        public double OfferPx { get; set; }
        public double OfferSize { get; set; }
        public double CumQty { get; set; }
        public double AvgPx { get; set; }
        public string StatusDesc { get; set; }
        public long OrdRejReason { get; set; }

        [DataMember(Name = "TransactionStatus")]
        public string TransactionStatusString
        {
            get => _transactionStatusString;
            set
            {
                _transactionStatusString = value;
                TransactionStatus = long.Parse(_transactionStatusString);
            }
        }
        private string _transactionStatusString;

        [IgnoreDataMember]
        public long TransactionStatus { get; set; }

        public string TimeInForce { get; set; }
        public string Text { get; set; }
        public string ID { get; set; }
        public string Symbol { get; set; }
        public string SymbolSfx { get; set; }
        public DateTime TransactTime { get; set; }
        public string ClientID { get; set; }
        public string LocateType { get; set; }
        public string BoothID { get; set; }
        public string Account { get; set; }
        public string OriginatingUserDesc { get; set; }
        public double EtbQty { get; set; }
    }
}