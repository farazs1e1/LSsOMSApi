using OMSServices.Data;

namespace OMSServices.Models
{
    public class CreateOrderResponse
    {
        public bool Success { get; set; }
        public bool Confirmation { get; set; }
        public string Message { get; set; }
        public ServerResponse ServerResponse { get; set; }
    }
}
