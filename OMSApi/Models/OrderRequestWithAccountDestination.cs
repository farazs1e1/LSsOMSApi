using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;

namespace OMSApi.Models
{
    public class OrderRequestInt : OrderRequest
    {
        //[StaticDataValidation(QueryType.Account, ErrorMessage = "Invalid account selected")]
        //public string Account { get; set; }

        internal override BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            BindableOEMessage bindableOEMessage = base.ToBOEMsg(boothID, originatingUserDesc);
            //bindableOEMessage.Account = Account;
            //bindableOEMessage.Destination = Destination;
            return bindableOEMessage;
        }
    }
}
