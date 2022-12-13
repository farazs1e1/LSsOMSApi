using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class OrderRequest : IValidatableObject
    {
        [StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid account selected")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Side field is required"), StaticDataValidation(QueryType.Side, ErrorMessage = "Invalid side selected")]
        public string Side { get; set; }

        public decimal OrderQty { get; set; }

        [Required(ErrorMessage = "Symbol Not Provided!"), RegularExpression(@"^[A-Z0-9]{1,9}(\.[A-Z0-9]{1,9})?(-[A-Z0-9]{1,4})?$", ErrorMessage = "Invalid symbol entered")]
        public string Symbol { get; set; }

        [Required(ErrorMessage = "OrderType field is required"), StaticDataValidation(QueryType.OrdType, ErrorMessage = "Invalid order type selected")]
        public string OrdType { get; set; }

        [RegularExpression("^\\d+[,.]?\\d{0,}$", ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }

        public decimal StopPx { get; set; }

        [Required(ErrorMessage = "TimeInForce field is required"), StaticDataValidation(QueryType.TIF, ErrorMessage = "Invalid time in force selected")]
        public string TimeInForce { get; set; }
        public string ComplianceID { get; set; }

        public string LocateID { get; set; }
        public string ContactName { get; set; }
        public bool LocateRequired { get; set; }
        public double LocateRate { get; set; }

        [StaticDataValidation(QueryType.Destination, ErrorMessage = "Invalid destination selected")]
        public string Destination { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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

        internal virtual BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
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
