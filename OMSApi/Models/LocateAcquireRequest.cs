using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;
using OMSServices.Utils;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OMSApi.Models
{
    public class LocateAcquireRequest : IValidatableObject
    {
        public decimal OrderQty { get; set; }

        [Required, RegularExpression(@"^[A-Z0-9]{1,9}(\.[A-Z0-9]{1,9})?(-[A-Z0-9]{1,4})?$", ErrorMessage = "Invalid symbol entered")]
        public string Symbol { get; set; }

        [Required]
        public string QuoteReqID { get; set; }

        [Required(ErrorMessage = "TimeInForce field is required"), StaticDataValidation(QueryType.LocateTIF, ErrorMessage = "Invalid time in force selected")]
        public string TimeInForce { get; set; }

        [StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid account selected")]
        public string Account { get; set; }

        public string ClientId { get; set; }

        /// <summary>
        /// When 0: will respond back without waiting for Locate-acquire status.
        /// When 1: will respond back only when Locate-acquire status has been received.
        /// </summary>
        public int Synchronous { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (OrderQty < 0 || OrderQty > Globals.MaxAllowed_Quantity)
                yield return new ValidationResult("Invalid quantity. Out of range.", new[] { nameof(OrderQty) });
        }

        internal BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            var oEMessage = new BindableOEMessage
            {
                BoothID = boothID,
                OriginatingUserDesc = originatingUserDesc,
                CompleteQty = OrderQty,
                Symbol = Symbol,
                QuoteReqID = QuoteReqID,
                OrdType = "1",
                TimeInForce = TimeInForce,
                MessageTag = 510,
                SubMessageTag = 452,
                Side = "1",
                Account = Account,
                ClientID = ClientId,
                Synchronous = Synchronous,
            };
            return oEMessage;
        }
    }
}
