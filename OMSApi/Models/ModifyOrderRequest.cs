using OMSServices.Models;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class ModifyOrderRequest : IValidatableObject
    {
        public decimal OrderQty { get; set; }

        [RegularExpression("^\\d+[,.]?\\d{0,}$", ErrorMessage = "Invalid price")]
        public decimal Price { get; set; }

        [Required, RegularExpression("^[1-9]\\d*$", ErrorMessage = "Invalid orderID")]
        public long QOrderID { get; set; }

        [Required, RegularExpression("^[1-4]$", ErrorMessage = "Invalid orderType")]
        public string OrdType { get; set; }
        public string ComplianceID { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderQty <= 0)
                yield return new ValidationResult("Invalid quantity", new[] { nameof(OrderQty) });
        }

        internal BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            var oEMessage = new BindableOEMessage
            {
                BoothID = boothID,
                OriginatingUserDesc = originatingUserDesc,
                CompleteQty = OrderQty,
                MessageTag = 510,
                SubMessageTag = 71,
                OrdType = OrdType,
                QOrderID = QOrderID,
                ComplianceID = ComplianceID,
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
