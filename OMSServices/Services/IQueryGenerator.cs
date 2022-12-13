using OMSServices.Data;
using OMSServices.Enum;
using System.Collections.Generic;
using System.Dynamic;

namespace OMSServices.Services
{
    public interface IQueryGenerator
    {
        ExpandoObject GetOneTimeQuery(QueryGeneratorArguments arguments);
        ExpandoObject GetOneTimeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, bool fallback = false, string symbol = null);
        ExpandoObject GetOneTimeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, string symbol);
        ExpandoObject GetOneTimeQueryObjectForTraderRelation(QueryType queryType, string userDesc, string boothId, bool fallback);

        bool HasContinuousQuery(QueryType queryType);
        ExpandoObject GetContinuousQuery(QueryType queryType);
        ExpandoObject GetContinuousQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, bool fallback = false, string symbol = null);
        ExpandoObject GetContinuousQuery(QueryGeneratorArguments arguments);

        ExpandoObject GetUnsubscribeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts);
        ExpandoObject GetUnsubscribeQuery(QueryType queryType);
        ExpandoObject GetUnsubscribeQuery(QueryGeneratorArguments arguments);
        ExpandoObject GetUnsubscribeQueryObjectForOverallData(QueryType queryType);

        ExpandoObject GetSelectAllQueryObject(QueryType queryType);
        ExpandoObject GetSubscribeQueryObjectForOverallData(QueryType queryType);
    }
}
