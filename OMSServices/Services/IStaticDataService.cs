using OMSServices.Enum;
using OMSServices.Models;
using System.Threading.Tasks;

namespace OMSServices.Services
{
    public interface IStaticDataService
    {
        Task<ResultDataObject<T>> GetStaticDataAsync<T>(QueryType queryType, string userDesc, string boothID, string userIdentifier) where T : class;
        Task<ResultDataObject<T>> GetStaticDataEasyToBorrowAsync<T>(string userDesc, string accountValue, string symbol) where T : class;
    }
}
