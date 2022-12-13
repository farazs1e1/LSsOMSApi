using Client.Communication;
using Client.Communication.Interfaces;
using OMSServices.Enum;
using OMSServices.Querry;
using OMSServices.Services;
using OMSServices.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Data
{
    public static class StaticData
    {
        private static readonly object _boothEndpoints_Lock = new();
        private static IDictionary<string, string> _boothEndpoints = null;

        private static IQueryGenerator _queryGenerator;

        public static void LoadAll(IQueryGenerator queryGenerator)
        {
            _queryGenerator = queryGenerator ?? throw new ArgumentNullException(nameof(queryGenerator));

            FetchAndSetBoothEndpoints();
            SubscribeForBoothEndpointsChanges();
        }

        public static void UnLoadAll()
        {
            UnSubscribeForBoothEndpointsChanges();
        }

        public static string GetBoothEndpoint(string boothId)
        {
            if (_boothEndpoints is null)
            {
                FetchAndSetBoothEndpoints();
            }
            return _boothEndpoints[boothId];
        }

        #region PRIVATE METHODS

        private static void FetchAndSetBoothEndpoints()
        {
            lock (_boothEndpoints_Lock)
            {
                if (_boothEndpoints != null)
                {
                    return;
                }
                var queryObject = _queryGenerator.GetSelectAllQueryObject(QueryType.Booths);
                ExpandoObject booths = EndpointHelper.GetStaticDataEndpoint().Send(queryObject).Wait();

                _boothEndpoints = booths.Get_Value<List<object>>("EventData").ToDictionary(x => x.GetValue<string>("BoothID"), x => x.GetValue<string>("VtID"));
            }
        }

        private static void SubscribeForBoothEndpointsChanges()
        {
            ExpandoObject queryObject = _queryGenerator.GetSubscribeQueryObjectForOverallData(QueryType.Booths);

            var endpoint = EndpointHelper.GetStaticDataEndpoint();
            _ = endpoint
                .Send(queryObject)
                .Where(x => x.Get_Value<long>("EventType") != (long)EventType.CurrentState)
                .Do(_ =>
                {
                    lock (_boothEndpoints_Lock)
                    {
                        _boothEndpoints = null;
                    }
                }).Subscribe();
        }

        private static void UnSubscribeForBoothEndpointsChanges()
        {
            ExpandoObject queryObject = _queryGenerator.GetUnsubscribeQueryObjectForOverallData(QueryType.Booths);
            EndpointHelper.GetStaticDataEndpoint().Send(queryObject);
        }

        #endregion
    }
}
