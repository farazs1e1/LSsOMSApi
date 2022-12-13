using OMSServices.Enum;
using OMSServices.Querry;
using System.Collections.Generic;

namespace OMSServices.Data
{
    internal static class QueryDataCollection
    {
        public static readonly IReadOnlyDictionary<QueryType, QueryData> Default = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Positions, new QueryData() { Columns = "*", Provider = "Positions", Cache = "Positions", Table = "POSITION", Condition = "( PositionOperation IN (1, 2) )", Predicate = "( PositionOperation IN (1, 2) )", ApplyAccountsConstraints = true } },

            { QueryType.Orders, new QueryData() { Columns = "*", Provider = "Orders", Cache = "Orders", Table = "\"ORDER\"", Condition = " ( ISOPTIONTRADE = false ) ", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.OptionOrders, new QueryData() { Columns = "*", Provider = "Orders", Cache = "Orders", Table = "\"ORDER\"", Condition = " ( ISOPTIONTRADE = true ) ", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.OpenOrders, new QueryData() { Columns = "*", Provider = "Orders", Cache = "Orders", Table = "\"ORDER\"", Condition = "( StatusDesc NOT IN ('Filled', 'Cancelled', 'Rejected', 'Expired','Manual Cxl') )", Predicate = "( !(StatusDesc IN(\"Filled\", \"Cancelled\", \"Rejected\", \"Expired\",\"Manual Cxl\")) )", ApplyAccountsConstraints = true } },

            { QueryType.Executions, new QueryData() { Columns = "*", Provider = "Executions", Cache = "Executions", Table = "EXECUTION", Condition = "", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.BuyingPower, new QueryData() { Columns = "*", Provider = "Orders", Cache = "BuyingPowers", Table = "BUYINGPOWER", Condition = "", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.Locates, new QueryData() { Columns = "*", Provider = "Locates", Cache = "Locates", Table = "LOCATEDETAILS", Condition = "", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.LocateSummary, new QueryData() { Columns = "*", Provider = "Locates", Cache = "LocateSummary", Table = "LOCATESUMMARY", Condition = "", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.LocateSummaryWithSymbol, new QueryData() { Columns = "*", Provider = "Locates", Cache = "LocateSummary", Table = "LOCATESUMMARY", Condition = "SYMBOLWITHOUTSFX = [Symbol]", Predicate = "", ApplyAccountsConstraints = true } },

            { QueryType.NetLimitSummary, new QueryData() { Columns = "*", Provider = "BuyingPowerProvider", Cache = "NetLimitSummary", Table = "NETLIMITSUMMARY", Condition = "", Predicate = "" } },

            { QueryType.AuditTrail, new QueryData() { Columns = "*", Provider = "OrderAuditTrail", Cache = "AuditTrail", Table = "AUDITTRAIL", Condition = "", Predicate = "" } },

            { QueryType.Side, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Side", Table = "SIDE a, \"TraderSides\".TRADERRELATIONWITH b", Condition = "a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Destination", Table = "DESTINATIONDETAILS a, \"TraderDestination\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Account", Table = "ACCOUNTDETAILS a, \"TraderAccount\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "TIF", Table = "TIMEINFORCE a, \"TraderTIF\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "CommType", Table = "COMMTYPE a, \"TraderCommType\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "OrdType", Table = "ORDERTYPE a, \"TraderOrderType\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.LocateTIF, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "LocateTIF", Table = "TIMEINFORCE", Condition = "", Predicate = "" } },

            { QueryType.MktTopPerfCateg, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "MarketTopPerformanceCategories", Table = "MARKETTOPPERFCATG", Condition = "", Predicate = "" } },

            { QueryType.MktTopPerfExchange, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "MarketTopPerformanceExchanges", Table = "MARKETTOPPERFEXCHANGE", Condition = "", Predicate = "" } },

            { QueryType.TimeZone, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "CustomTimeZoneInfo", Table = "CUSTOMTIMEZONEINFO a, \"TraderTimeZones\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = [UserDesc] AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.ETBHTB, new QueryData() { Columns = "*", Provider = "EtbHtb", Cache = "EasyToBorrow", Table = "EASYTOBORROW", Condition = "BOOTHID = [BoothID] AND SYMBOL = [Symbol]", Predicate = "" } },
        };


        public static readonly IReadOnlyDictionary<QueryType, QueryData> Fallback = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Side, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Side", Table = "SIDE a, \"TraderSides\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.TimeZone, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "CustomTimeZoneInfo", Table = "CUSTOMTIMEZONEINFO a, \"TraderTimeZones\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Destination", Table = "DESTINATIONDETAILS a, \"TraderDestination\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "Account", Table = "ACCOUNTDETAILS a, \"TraderAccount\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "TIF", Table = "TIMEINFORCE a, \"TraderTIF\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "CommType", Table = "COMMTYPE a, \"TraderCommType\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "a.*", Provider = "StaticData", Cache = "OrdType", Table = "ORDERTYPE a, \"TraderOrderType\".TRADERRELATIONWITH b", Condition = $"a.VALUE = b.RECORDID AND b.USERNAME = '' AND b.BOOTHID = [BoothID]", Predicate = "" } },
        };


        public static readonly IReadOnlyDictionary<QueryType, QueryData> TraderRelation = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Side, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderSides", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderDestination", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderAccount", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderTIF", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderCommType", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderOrderType", Table = "TRADERRELATIONWITH", Condition = "USERNAME = [UserDesc] AND BOOTHID = [BoothID]", Predicate = "" } },
        };


        public static readonly IReadOnlyDictionary<QueryType, QueryData> Continuous = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Side, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderSides", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderDestination", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderAccount", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderTIF", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderCommType", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderOrderType", Table = "TRADERRELATIONWITH", Condition = "BOOTHID != ''", Predicate = "" } },
        };


        public static readonly IReadOnlyDictionary<QueryType, QueryData> SelectAll = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Side, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "Side", Table = "SIDE", Condition = "", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "Destination", Table = "DESTINATIONDETAILS", Condition = "", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "Account", Table = "ACCOUNTDETAILS", Condition = "", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TIF", Table = "TIMEINFORCE", Condition = "", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "CommType", Table = "COMMTYPE", Condition = "", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "OrdType", Table = "ORDERTYPE", Condition = "", Predicate = "" } },

            { QueryType.Booths, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "Booths", Table = "BOOTH", Condition = "", Predicate = "" } },
        };


        public static readonly IReadOnlyDictionary<QueryType, QueryData> TraderRelationFallback = new Dictionary<QueryType, QueryData>()
        {
            { QueryType.Side, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderSides", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Destination, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderDestination", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.Account, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderAccount", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.TIF, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderTIF", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.CommType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderCommType", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },

            { QueryType.OrdType, new QueryData() { Columns = "*", Provider = "StaticData", Cache = "TraderOrderType", Table = "TRADERRELATIONWITH", Condition = "USERNAME = '' AND BOOTHID = [BoothID]", Predicate = "" } },
        };
    }
}
