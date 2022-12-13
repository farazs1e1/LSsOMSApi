using OMSServices.Enum;
using OMSServices.Utils;
using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionOptionOrder : SubscriptionOrder
    {
        public string OptionSymbol { get; set; }
        public double StrikePrice { get; set; }
        public int MaturityDay { get; set; }
        public string MaturityMonthYear { get; set; }

        [DataMember(Name = "PutOrCall")]
        public int PutOrCallInt
        {
            get => _PutOrCallInt;
            set
            {
                _PutOrCallInt = value;
                PutOrCall = (PutCall)(char)_PutOrCallInt;
            }
        }
        private int _PutOrCallInt;

        [IgnoreDataMember]
        public PutCall PutOrCall { get; set; }

        [DataMember(Name = "CustomerOrFirm")]
        public int CustomerOrFirmInt
        {
            get => _CustomerOrFirm;
            set
            {
                _CustomerOrFirm = value;
                CustomerOrFirm = (CustomerFirm)(char)_CustomerOrFirm;
            }
        }
        private int _CustomerOrFirm;

        [IgnoreDataMember]
        public CustomerFirm CustomerOrFirm { get; set; }


        [DataMember(Name = "CoveredOrUncovered")]
        public int CoveredOrUncoveredInt
        {
            get => _CoveredOrUncoveredInt;
            set
            {
                _CoveredOrUncoveredInt = value;
                CoveredOrUncovered = (CoveredUnCovered)(char)_CoveredOrUncoveredInt;
            }
        }
        private int _CoveredOrUncoveredInt;

        [IgnoreDataMember]
        public CoveredUnCovered CoveredOrUncovered { get; set; }

        public string Cmta { get; set; }
        public string ExecBroker { get; set; }

        [DataMember(Name = "OpenClose")]
        public object OpenCloseBoxed
        {
            get => _openCloseBoxed;
            set
            {
                _openCloseBoxed = value;
                OpenClose = EnumsHelper.ConvertToOpenClose(_openCloseBoxed);
            }
        }
        private object _openCloseBoxed;

        [IgnoreDataMember]
        public OpenClose OpenClose { get; set; }
    }
}
