using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class OptionOrderRequest : OrderRequest, IValidatableObject
    {
        private int _maturityDay;
        private string _maturityMonthYear;

        [Required(ErrorMessage = "ExpiryDate Not Provided!")]
        public DateTime ExpiryDate
        {
            get => _expiryDate;
            set
            {
                _expiryDate = value;
                _maturityDay = _expiryDate.Day;
                _maturityMonthYear = _expiryDate.ToString("yyyyMM");
            }
        }
        private DateTime _expiryDate = DateTime.Now.Date;

        [Required(ErrorMessage = "StrikePrice Not Provided!")]
        public decimal StrikePrice { get; set; }

        [Required(ErrorMessage = "PutOrCall Not Provided!")]
        public PutCall PutOrCall { get; set; }

        [Required(ErrorMessage = "CustomerOrFirm Not Provided!")]
        public CustomerFirm CustomerOrFirm { get; set; }

        [Required(ErrorMessage = "CoveredOrUncovered Not Provided!")]
        public CoveredUnCovered CoveredOrUncovered { get; set; }

        [Required(ErrorMessage = "Cmta Not Provided!")]
        public string Cmta { get; set; } = "";

        [Required(ErrorMessage = "OpenClose Not Provided!")]
        public OpenClose OpenClose { get; set; } = OpenClose.OPEN;

        public string ExecBroker { get; set; } = "";
        public char OptAttribute { get; set; }
        public string UnderlyingSymbol { get; set; }
        public string OptionSymbol { get; set; } = "";

        public new IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StrikePrice <= 0 || StrikePrice > Globals.MaxAllowed_Price)
                yield return new ValidationResult("Invalid strike price. Out of range.", new[] { nameof(StrikePrice) });

            if (OrderQty <= 0 || OrderQty > Globals.MaxAllowed_Quantity)
                yield return new ValidationResult("Invalid quantity. Out of range.", new[] { nameof(OrderQty) });

            if (LocateRate < 0 || LocateRate > Globals.MaxAllowed_Price)
                yield return new ValidationResult("Invalid locate rate. Out of range.", new[] { nameof(LocateRate) });

            if (OrdType != null)
            {
                if (OrdType == "1")
                {
                    if (Price > 0)
                        yield return new ValidationResult("Market order does not take price field.", new[] { nameof(Price) });
                    if (StopPx > 0)
                        yield return new ValidationResult("Market order does not take stop price field.", new[] { nameof(StopPx) });
                }
                else if (OrdType == "2")
                {
                    if (Price <= 0 || Price > Globals.MaxAllowed_Price)
                        yield return new ValidationResult("Invalid price. Out of range.", new[] { nameof(Price) });

                    if (StopPx > 0)
                        yield return new ValidationResult("Limit order does not take stop price field.", new[] { nameof(StopPx) });
                }
                else if (OrdType == "3")
                {
                    if (Price > 0)
                        yield return new ValidationResult("Stop order does not take price field.", new[] { nameof(Price) });

                    if (StopPx <= 0 || StopPx > Globals.MaxAllowed_Price)
                        yield return new ValidationResult("Invalid stop price. Out of range.", new[] { nameof(StopPx) });
                }
                else if (OrdType == "4")
                {
                    if (Price <= 0 || Price > Globals.MaxAllowed_Price)
                        yield return new ValidationResult("Invalid price. Out of range.", new[] { nameof(Price) });
                    if (StopPx <= 0 || StopPx > Globals.MaxAllowed_Price)
                        yield return new ValidationResult("Invalid stop price. Out of range.", new[] { nameof(StopPx) });
                }
            }
        }

        internal override BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            if (string.IsNullOrEmpty(UnderlyingSymbol))
            {
                UnderlyingSymbol = Symbol;
            }
            if (string.IsNullOrEmpty(OptionSymbol))
            {
                OptionSymbol = Symbol;
            }
            var oEMessage = new BindableOEMessage
            {
                BoothID = boothID,
                OriginatingUserDesc = originatingUserDesc,
                Side = Side,
                Symbol = Symbol,
                OrdType = OrdType,
                StopPx = StopPx,
                TimeInForce = TimeInForce,
                MessageTag = (short)(OrderQty.HasFractionalPart() ? 510 : 68),
                SubMessageTag = (short)(OrderQty.HasFractionalPart() ? 573 : 0),
                CompleteQty = OrderQty,
                Account = Account,
                ComplianceID = ComplianceID,
                Groupid = LocateID,
                Contactname = ContactName,
                LocateReqd = LocateRequired,
                LocateRate = LocateRate,

                //200-207
                MaturityMonthYear = _maturityMonthYear, // extract exp date
                PutOrCall = ((char)PutOrCall).ToString(), // req sell/buy
                StrikePrice = StrikePrice, // req > 0
                CoveredOrUncovered = ((char)CoveredOrUncovered).ToString(), //req- postion yes then covered
                CustomerOrFirm = ((char)CustomerOrFirm).ToString(), //req 
                MaturityDay = _maturityDay, // extract exp date
                OptAttribute = OptAttribute, // optional
                                             //SecurityExchange

                UnderlyingSymbol = UnderlyingSymbol, //pvt set symbol
                OptionSymbol = OptionSymbol, //optional -- bydefault sybmol
                ExpiryDate = ExpiryDate, //requied -- user define
                OpenClose = (char)OpenClose, //--- ask
                IsOptionTrade = true, // internal
                OptionsFields = $"4={Cmta}|5={OptionSymbol}|77={(char)OpenClose}|200={_maturityMonthYear}|201={(char)PutOrCall}|202={StrikePrice}|203={(char)CoveredOrUncovered}|204={(char)CustomerOrFirm}|205={_maturityDay}|", //internal
                Cmta = Cmta, //req - anything
                ExecBroker = ExecBroker, // optional - its giveup
                Price = Price,
                Destination = Destination,
            };

            if (OrderQty.HasFractionalPart())
            {
                oEMessage.DollarValue = Price;
            }
            return oEMessage;
        }
    }
}

