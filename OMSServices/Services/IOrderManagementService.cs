using OMSServices.Models;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IOrderManagementService
    {
        Task<CreateOrderResponse> CreateOrderAsync(BindableOEMessage order, string userIdentifier);
        Task<object> UpdateOrderAsync(BindableOEMessage order, string userIdentifier);
        object CancelOrderAsync(long QOrderID, string boothId, string originatingUserDesc);
        ResultDto CancelOrderV3(long qOrderId, string boothId, string originatingUserDesc);
    }
}
