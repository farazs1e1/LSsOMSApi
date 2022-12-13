using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class LocateRequest : IValidatableObject
    {
        public decimal OrderQty { get; set; }

        public decimal Price { get; set; }

        [Required, RegularExpression(@"^[A-Z0-9]{1,9}(\.[A-Z0-9]{1,9})?(-[A-Z0-9]{1,4})?$", ErrorMessage = "Invalid symbol entered")]
        public string Symbol { get; set; }

        [StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid account selected")]
        public string Account { get; set; }

        public string ClientId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderQty <= 0 || OrderQty > Globals.MaxAllowed_Quantity)
                yield return new ValidationResult("Invalid quantity. Out of range.", new[] { nameof(OrderQty) });

            if (Price < 0 || Price > Globals.MaxAllowed_Price)
                yield return new ValidationResult("Invalid price. Out of range.", new[] { nameof(Price) });
        }

        internal BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            var oEMessage = new BindableOEMessage
            {
                BoothID = boothID,
                OriginatingUserDesc = originatingUserDesc,
                CompleteQty = OrderQty,
                Symbol = Symbol,
                MessageTag = 510,
                SubMessageTag = 82,
                Side = "5",
                Account = Account,
                ClientID = ClientId,
                Price = Price,
            };

            if (OrderQty.HasFractionalPart())
            {
                oEMessage.DollarValue = Price;
            }
            return oEMessage;
        }
    }
}
