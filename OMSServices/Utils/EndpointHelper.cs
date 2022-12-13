using Client.Communication.Interfaces;
using Client.Communication;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMSServices.Utils
{
    public static class EndpointHelper
    {
        public static ICommunicationEndPoint<ExpandoObject, ExpandoObject> GetStaticDataEndpoint()
        {
            return EndPointManager.Instance.GetEndPoint("StaticDataEndPoint") as ICommunicationEndPoint<ExpandoObject, ExpandoObject>;
        }

        public static IRequestPullEndPoint<ExpandoObject, ExpandoObject> GetLogonEndpoint()
        {
            return EndPointManager.Instance.GetEndPoint("Logon") as IRequestPullEndPoint<ExpandoObject, ExpandoObject>;
        }
    }
}
