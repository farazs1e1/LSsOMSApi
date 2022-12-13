using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace OMSServices.Models
{
    public class BindableOEMessage
    {
        public static PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(BindableOEMessage));

        public string Side { get; set; }

        private decimal _CompleteQty = 0;
        public decimal CompleteQty
        {
            get { return _CompleteQty; }
            set
            {
                _CompleteQty = value;

                OrderQty = (long)_CompleteQty;
                FractionalQty = (long)((_CompleteQty - OrderQty) * 1000000);
            }
        }

        public long OrderQty { get; set; }
        public long FractionalQty { get; set; }

        private string _Symbol = "";
        public string Symbol
        {
            get { return _Symbol; }
            set
            {
                if (value == null) return;
                if (_Symbol == value) return;
                _Symbol = value;
                int sfxIndex = _Symbol.IndexOf('.');
                if (sfxIndex != -1)
                {
                    SymbolWithoutSfx = _Symbol.Substring(0, sfxIndex);
                    SymbolSfx = _Symbol.Substring(sfxIndex + 1);
                }
                else
                {
                    SymbolWithoutSfx = _Symbol;
                    SymbolSfx = "";
                }
            }
        }
        [System.Xml.Serialization.XmlIgnore]
        public string SymbolWithoutSfx { get; private set; }

        [System.Xml.Serialization.XmlIgnore]
        public string SymbolSfx { get; private set; }
        public string OrdType { get; set; } = "2";
        public decimal DollarValue { get; set; }
        public decimal Price { get; set; }
        public decimal StopPx { get; set; }
        public string Account { get; set; }
        public string Destination { get; set; }
        public string TimeInForce { get; set; }
        public long QOrderID { get; set; }
        public string ComplianceID { get; set; }
        public string CommType { get; set; } = "\u0000";
        public string QuoteReqID { get; set; } = "C0";
        public DateTime GTDDate { get; set; } = DateTime.Now.Date;
        public char Rule80A { get => 'P'; }


        //Header Fields
        public string BoothID { get; set; }
        public string ClientID { get; set; }
        public short MessageTag { get; set; }
        public short SubMessageTag { get; set; }
        public string OriginatingUserDesc { get; set; }
        public string Text { get; set; }
        public string DestinationDesc { get; set; }
        public int Synchronous { get; set; }

        public int MsgType { get => MessageTag; }
        public char ByteOrder { get => (char)1; }
        public char BoothIdent { get => 'B'; }
        public int ModifySend { get => 1; }
        public int ModifyParentQty { get => 1; }
        public string HdrBoothID { get => BoothID; }
        public string TargetLocationID { get => BoothID; }
        public string Groupid { get; set; }
        public string Contactname { get; set; }
        public bool LocateReqd { get; set; }
        public double LocateRate { get; set; }
        public string MaturityMonthYear { get; set; }
        public string ExecBroker { get; set; }
        public string Cmta { get; set; }
        public string OptionsFields { get; set; }
        public bool IsOptionTrade { get; set; }
        public char OpenClose { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OptionSymbol { get; set; }
        public char OptAttribute { get; set; }
        public int MaturityDay { get; set; }
        public string CustomerOrFirm { get; set; }
        public string CoveredOrUncovered { get; set; }
        public decimal StrikePrice { get; set; }
        public string PutOrCall { get; set; }
        public string UnderlyingSymbol { get; set; }
    }

    public static class BindableOEMessageToDictionary
    {
        public static IDictionary<string, object> ToDictionary(this BindableOEMessage order)
        {
            var dictionary = new ExpandoObject() as IDictionary<string, object>;
            foreach (PropertyDescriptor prop in BindableOEMessage.properties)
                dictionary.Add(prop.Name, prop.GetValue(order));
            return dictionary;
        }
    }
}
