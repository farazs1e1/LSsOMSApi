using System.Runtime.Serialization;

namespace OMSServices.Models
{
    public class SubscriptionOptionOrderInt : SubscriptionOptionOrder
    {
        [DataMember(Name = "ID")]
        public string Id { get; set; }

        public string Time { get; set; }
    }
}
