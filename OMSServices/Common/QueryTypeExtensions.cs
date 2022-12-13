using OMSServices.Enum;

namespace OMSServices.Common
{
    static class QueryTypeExtensions
    {
        public static string GenerateCacheKeyForStaticData(this QueryType queryType, string userIdentifier) => $"{queryType}_{userIdentifier}";
        public static string GenerateCacheKeyForFallbackStaticData(this QueryType queryType, string boothId) => $"{queryType}_{boothId}_Fallback";
        public static string GenerateCacheKeyForUnfilteredStaticData(this QueryType queryType) => $"UNFILTERED_STATIC_DATA_{queryType}";
        public static string GenerateCacheKeyForEtbHtb(string account, string symbol) => $"{QueryType.ETBHTB}_{account}_{symbol}";
        
        public static string GenerateQueryKey(string userDescription, string boothId, QueryType queryType) => $"{userDescription}_{boothId}_{queryType}";
        public static string GenerateFallbackQueryKey(string queryKey) => $"{queryKey}_Fallback";
    }
}
