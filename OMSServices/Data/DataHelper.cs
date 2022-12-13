using System.Collections.Generic;
using System.Dynamic;

namespace OMSServices.Data
{
    internal class DataHelper
    {
        public static ExpandoObject WrapBOEMessage(IDictionary<string, object> orderDictionary, string boothId)
        {
            var data = new ExpandoObject();
            data.TryAdd("EndPointName", StaticData.GetBoothEndpoint(boothId));
            data.TryAdd("OrderData", orderDictionary);

            return data;
        }
    }
}
