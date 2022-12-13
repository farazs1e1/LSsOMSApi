using OMSApi.Attributes;
using OMSServices.Enum;
using OMSServices.Models;

namespace OMSApi.Models
{
    public class OptionOrderRequestInt : OptionOrderRequest
    {
        //[StaticDataValidation(QueryType.Destination, ErrorMessage = "Invalid destination selected")]
        //public string Destination { get; set; }

        internal override BindableOEMessage ToBOEMsg(string boothID, string originatingUserDesc)
        {
            BindableOEMessage bindableOEMessage = base.ToBOEMsg(boothID, originatingUserDesc);
            //bindableOEMessage.Destination = Destination;
            return bindableOEMessage;
        }
    }
}
