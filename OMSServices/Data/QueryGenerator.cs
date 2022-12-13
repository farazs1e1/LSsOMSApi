using Microsoft.Extensions.Logging;
using OMSServices.Common;
using OMSServices.Data;
using OMSServices.Enum;
using OMSServices.Implementation;
using OMSServices.Services;
using System.Collections.Generic;
using System.Dynamic;

namespace OMSServices.Querry
{
    public class QueryGenerator : IQueryGenerator
    {
        private readonly ILogger<QueryGenerator> _logger;
        private readonly JwtTokenProvider _jwtTokenProvider;

        public QueryGenerator(ILogger<QueryGenerator> logger, JwtTokenProvider jwtTokenProvider)
        {
            _logger = logger;
            _jwtTokenProvider = jwtTokenProvider;
        }

        public ExpandoObject GetOneTimeQuery(QueryGeneratorArguments arguments)
        {
            arguments.RequestType = RequestType.Query;
            return GetQueryCore(arguments);
        }

        public ExpandoObject GetOneTimeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, bool fallback = false, string symbol = null)
        {
            return GetQueryCore(new QueryGeneratorArguments
            {
                QueryType = queryType,
                UserDesc = userDesc,
                BoothID = boothID,
                Accounts = accounts,
                Fallback = fallback,
                Symbol = symbol,
                RequestType = RequestType.Query,
            });
        }

        public ExpandoObject GetOneTimeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, string symbol)
        {
            return GetQueryCore(new QueryGeneratorArguments
            {
                QueryType = queryType,
                UserDesc = userDesc,
                BoothID = boothID,
                Accounts = accounts,
                Symbol = symbol,
                RequestType = RequestType.Query,
            });
        }

        public ExpandoObject GetContinuousQuery(QueryGeneratorArguments arguments)
        {
            arguments.RequestType = RequestType.QueryContinuous;
            return GetQueryCore(arguments);
        }

        public ExpandoObject GetContinuousQuery(QueryType queryType)
        {
            return GetContinuousQueryCore(queryType, RequestType.QueryContinuous, null, null, null);
        }

        public ExpandoObject GetContinuousQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts, bool fallback = false, string symbol = null)
        {
            return GetQueryCore(new QueryGeneratorArguments
            {
                QueryType = queryType,
                UserDesc = userDesc,
                BoothID = boothID,
                Accounts = accounts,
                Fallback = fallback,
                Symbol = symbol,
                RequestType = RequestType.QueryContinuous,
            });
        }

        public ExpandoObject GetUnsubscribeQuery(QueryType queryType)
        {
            return GetContinuousQueryCore(queryType, RequestType.Unsubscribe, null, null, null);
        }

        public ExpandoObject GetUnsubscribeQuery(QueryType queryType, string userDesc, string boothID, List<string> accounts)
        {
            return GetQueryCore(new QueryGeneratorArguments
            {
                QueryType = queryType,
                UserDesc = userDesc,
                BoothID = boothID,
                Accounts = accounts,
                RequestType = RequestType.Unsubscribe,
            });
        }

        public ExpandoObject GetUnsubscribeQuery(QueryGeneratorArguments arguments)
        {
            arguments.RequestType = RequestType.Unsubscribe;
            return GetQueryCore(arguments);
        }

        public ExpandoObject GetOneTimeQueryObjectForTraderRelation(QueryType queryType, string userDesc, string boothId, bool fallback)
        {
            QueryData queryData;
            string queryKey = QueryTypeExtensions.GenerateQueryKey(userDesc, boothId, queryType);

            if (fallback)
            {
                if (!QueryDataCollection.TraderRelationFallback.ContainsKey(queryType))
                    return null;
                queryData = QueryDataCollection.TraderRelationFallback[queryType];

                queryKey = QueryTypeExtensions.GenerateFallbackQueryKey(queryKey);
            }
            else
            {
                if (!QueryDataCollection.TraderRelation.ContainsKey(queryType))
                    return null;
                queryData = QueryDataCollection.TraderRelation[queryType];
            }

            return GetQueryObject(queryData, queryType, null, queryKey, userDesc, boothId, RequestType.Query);
        }

        public ExpandoObject GetSelectAllQueryObject(QueryType queryType)
        {
            return GetQueryObjectForOverallData(queryType, RequestType.Query);
        }

        public ExpandoObject GetSubscribeQueryObjectForOverallData(QueryType queryType)
        {
            return GetQueryObjectForOverallData(queryType, RequestType.QueryContinuous);
        }

        public ExpandoObject GetUnsubscribeQueryObjectForOverallData(QueryType queryType)
        {
            return GetQueryObjectForOverallData(queryType, RequestType.Unsubscribe);
        }

        public bool HasContinuousQuery(QueryType queryType)
        {
            return QueryDataCollection.Continuous.ContainsKey(queryType);
        }

        /// <summary>
        /// PRIVATE METHODS
        /// </summary>

        private ExpandoObject GetQueryObjectForOverallData(QueryType queryType, RequestType requestType)
        {
            if (!QueryDataCollection.SelectAll.ContainsKey(queryType))
            {
                return null;
            }

            string queryKey = QueryTypeExtensions.GenerateQueryKey("SELECT", "ALL", queryType);

            QueryData queryData = QueryDataCollection.SelectAll[queryType];

            return GetQueryObject(queryData, queryType, null, queryKey, null, null, requestType);
        }

        private ExpandoObject GetQueryCore(QueryGeneratorArguments args)
        {
            string queryKey = string.IsNullOrEmpty(args.CustomQueryKey) ? QueryTypeExtensions.GenerateQueryKey(args.UserDesc, args.BoothID, args.QueryType) : args.CustomQueryKey;

            QueryData queryData;
            if (args.Fallback)
            {
                if (!QueryDataCollection.Fallback.ContainsKey(args.QueryType))
                    return null;
                queryKey = QueryTypeExtensions.GenerateFallbackQueryKey(queryKey);
                queryData = QueryDataCollection.Fallback[args.QueryType];
            }
            else
            {
                if (!QueryDataCollection.Default.ContainsKey(args.QueryType))
                    return null;
                queryData = QueryDataCollection.Default[args.QueryType];
            }

            string orderByQuery = string.IsNullOrEmpty(args.OrderBy) ? string.Empty : $" Order By {args.OrderBy}";
            var (queryConditionWithAccounts, queryPredicateWithAccounts) = CreateQueryConditionAndPredicateWithAccounts(args.Accounts);

            dynamic request = new ExpandoObject();
            request.Provider = queryData.Provider;
            request.QueryType = args.QueryType;
            if (string.IsNullOrEmpty(args.UpdateQuery))
            {
                request.QueryObject = CreateQueryObject(
                    queryKey,
                    queryData.Columns,
                    queryData.Cache,
                    queryData.Table,
                    UpdateQueryCondition(queryConditionWithAccounts, queryData.Condition, args.UserDesc, args.BoothID, args.Symbol, queryData.ApplyAccountsConstraints) + orderByQuery,
                    UpdateQueryPredicate(queryPredicateWithAccounts, queryData.Predicate, queryData.ApplyAccountsConstraints),
                    args.RequestType,
                    args.QueryType
                );
            }
            else
            {
                string condition = queryData.Condition == "" ? args.UpdateQuery : queryData.Condition + " AND " + args.UpdateQuery;
                string predicate = queryData.Predicate == "" ? args.UpdateQuery.Replace("'", "\"") : queryData.Predicate + " AND " + args.UpdateQuery.Replace("'", "\"");
                request.QueryObject = CreateQueryObject(
                    queryKey,
                    queryData.Columns,
                    queryData.Cache,
                    queryData.Table,
                    UpdateQueryCondition(queryConditionWithAccounts, condition, args.UserDesc, args.BoothID, args.Symbol, queryData.ApplyAccountsConstraints) + orderByQuery,
                    UpdateQueryPredicate(queryPredicateWithAccounts, predicate, queryData.ApplyAccountsConstraints),
                    args.RequestType,
                    args.QueryType
                );
            }

            if (args.QueryType == QueryType.Positions)
            {
                dynamic innerRequest = request;
                request = new ExpandoObject();
                request.Provider = "PositionsWithMktData";
                request.QueryType = args.QueryType;
                request.QueryObject = innerRequest;
            }

            return request;
        }

        private ExpandoObject GetContinuousQueryCore(QueryType queryType, RequestType requestType, string userDesc, string boothID, List<string> accounts)
        {
            if (!QueryDataCollection.Continuous.ContainsKey(queryType))
            {
                return null;
            }

            string queryKey = QueryTypeExtensions.GenerateQueryKey(userDesc, boothID, queryType);

            QueryData queryData = QueryDataCollection.Continuous[queryType];

            return CreateQueryInfo(queryData, queryType, accounts, queryKey, userDesc, boothID, null, requestType);
        }

        private ExpandoObject CreateQueryInfo(QueryData queryData, QueryType queryType, List<string> accounts, string queryKey, string userDesc, string boothId, string symbol, RequestType requestType)
        {
            dynamic request = new ExpandoObject();
            request.Provider = queryData.Provider;
            request.QueryType = queryType;

            var (queryConditionWithAccounts, queryPredicateWithAccounts) = CreateQueryConditionAndPredicateWithAccounts(accounts);

            request.QueryObject = CreateQueryObject(
                queryKey,
                queryData.Columns,
                queryData.Cache,
                queryData.Table,
                UpdateQueryCondition(queryConditionWithAccounts, queryData.Condition, userDesc, boothId, symbol, queryData.ApplyAccountsConstraints),
                UpdateQueryPredicate(queryPredicateWithAccounts, queryData.Predicate, queryData.ApplyAccountsConstraints),
                requestType,
                queryType
            );

            return request;
        }

        private ExpandoObject GetQueryObject(QueryData queryData, QueryType queryType, List<string> accounts, string queryKey, string userDesc, string boothId, RequestType requestType)
        {
            var (queryConditionWithAccounts, queryPredicateWithAccounts) = CreateQueryConditionAndPredicateWithAccounts(accounts);

            dynamic queryResult = CreateQueryObject(
                queryKey,
                queryData.Columns,
                queryData.Cache,
                queryData.Table,
                UpdateQueryCondition(queryConditionWithAccounts, queryData.Condition, userDesc, boothId, null, queryData.ApplyAccountsConstraints),
                UpdateQueryPredicate(queryPredicateWithAccounts, queryData.Predicate, queryData.ApplyAccountsConstraints),
                requestType,
                queryType
            );

            return queryResult;
        }

        private ExpandoObject CreateQueryObject(string queryKey, string columns, string cache, string table, string queryCondition, string predicate, RequestType requestType, QueryType queryType)
        {
            dynamic queryObject = new ExpandoObject();
            queryObject.QueryKey = queryKey;
            queryObject.Query = $"SELECT {columns} FROM \"{cache}\".{table}";

            if (!string.IsNullOrEmpty(queryCondition))
            {
                queryObject.Query += $" WHERE {queryCondition}";
            }

            queryObject.Predicate = predicate;
            queryObject.Cache = cache;
            queryObject.RequestType = (int)requestType;

            if (_jwtTokenProvider.Enabled)
            {
                queryObject.JWTToken = _jwtTokenProvider.Token;
            }

            _logger.LogInformation("QueryKey: {QueryKey}, QueryType: {QueryType}, RequestType: {RequestType}, RawQuery: {RawQuery}", queryKey, queryType, requestType, (string)queryObject.Query);
            return queryObject;
        }

        /// <summary>
        ///  PRIVATE STATIC METHODS
        /// </summary>

        private static (string queryCondition, string queryPredicate) CreateQueryConditionAndPredicateWithAccounts(List<string> accounts)
        {
            string queryCondition = string.Empty;
            if (accounts != null && accounts.Count > 0)
            {
                queryCondition = $" ( Account IN ( ";

                foreach (var account in accounts)
                    queryCondition += $" '{account}',";

                queryCondition = queryCondition.TrimEnd(',');
                queryCondition += $" ) )";
            }

            string queryPredicate = queryCondition.Replace("'", "\"");
            return (queryCondition, queryPredicate);
        }

        private static string UpdateQueryCondition(string queryConditionWithAccounts, string queryCondition, string userDesc, string boothID, string symbol, bool apply)
        {
            queryCondition = ReplacePlaceholdersInCondition(queryCondition, userDesc, boothID, symbol);

            if (!apply)
                return queryCondition;

            if (string.IsNullOrEmpty(queryCondition))
                return queryConditionWithAccounts;

            if (string.IsNullOrEmpty(queryConditionWithAccounts))
                return queryCondition;

            return queryCondition + " AND " + queryConditionWithAccounts;
        }

        private static string UpdateQueryPredicate(string queryPredicateWithAccounts, string queryPredicate, bool apply)
        {
            if (!apply)
                return queryPredicate;

            if (string.IsNullOrEmpty(queryPredicate))
                return queryPredicateWithAccounts;

            if (string.IsNullOrEmpty(queryPredicateWithAccounts))
                return queryPredicate;

            return queryPredicate + " AND " + queryPredicateWithAccounts;
        }

        private static string ReplacePlaceholdersInCondition(string queryCondition, string userDesc, string boothID, string symbol)
        {
            if (queryCondition.Contains("[UserDesc]"))
            {
                queryCondition = queryCondition.Replace("[UserDesc]", "'" + userDesc + "'");
            }
            if (queryCondition.Contains("[BoothID]"))
            {
                queryCondition = queryCondition.Replace("[BoothID]", "'" + boothID + "'");
            }
            if (queryCondition.Contains("[Symbol]"))
            {
                queryCondition = queryCondition.Replace("[Symbol]", "'" + symbol + "'");
            }
            return queryCondition;
        }
    }
}
