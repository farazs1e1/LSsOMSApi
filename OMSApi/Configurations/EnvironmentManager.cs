using Client.Communication;
using Client.Communication.Configurations;
using Client.Providers;
using Client.Providers.ServiceProviders;
using Client.Providers.Utilities;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OMSApi.Configurations
{
    internal class EnvironmentManager
    {
        private static readonly object _lock = new object();
        private static readonly ILog logger = LogManager.GetLogger("EnvironmentManager");
        private static EnvironmentManager environmentManager;
        private static XmlSerializer ProtocolSerializer = new XmlSerializer(typeof(ProtocolChannelConfiguration), new[] { typeof(OEChannelConfiguration) });
        private static XmlSerializer endpointsSerializer = new XmlSerializer(typeof(EndPointConfiguration), new[]
        {
            typeof(QueryEndPointConfiguration),
            typeof(RequestPullEndPointConfiguration<string, ExpandoObject, ExpandoObject>),
            typeof(StaticDataEndPointConfiguration<string, ExpandoObject>),
            typeof(StreamableQueryEndPointConfiguration<string, ExpandoObject, ExpandoObject, ExpandoObject, long>),
            typeof(StreamableQueryEndPointConfiguration<string, Dictionary<uint, string>, ExpandoObject, ExpandoObject, long>),
            typeof(RequestBatchResponseEndPointConfiguration<string, Dictionary<uint, string>, ExpandoObject>)
        });

        private EnvironmentConfig EnvConfig;
        private List<ProtocolChannelConfiguration> protocolChannelConfigurations = new List<ProtocolChannelConfiguration>();
        private List<EndPointConfiguration> endpointsConfigurations = new List<EndPointConfiguration>();

        public static EnvironmentManager Instance
        {
            get
            {
                if (environmentManager == null)
                {
                    lock (_lock)
                    {
                        if (environmentManager == null)
                            environmentManager = new EnvironmentManager();
                    }
                }
                return environmentManager;
            }
        }

        public string GetEnvironmentAndLoadLogon(object Data)
        {
            try
            {
                EndPointManager.Instance.RemoveEndPoint("Logon");
                GlobalProvider.Instance.RemoveProvider("Logon");
                ProtocolChannelManager.Instance.ClearChannels();
                protocolChannelConfigurations.Clear();
                endpointsConfigurations.Clear();

                ExpandoObject response = Helper.SendDatatoServiceProvider("Config", Data) as ExpandoObject;

                string result = "";
                if (!ProcessServerResponse(response, ref result))
                {
                    return result;
                }
                //Have to parse configurations and then have to initialize endpoints
                var responseDictionary = response as IDictionary<string, object>;
                if (!responseDictionary.ContainsKey("Data"))
                    return "Unable to get configurations from server";

                var data = responseDictionary["Data"] as IDictionary<string, object>;
                if (!data.ContainsKey("EventData"))
                    return "Unable to get configurations from server";

                //This list will contain multiple environment as per configurations. For now we are using only first configuration. To enable multiple environment support have to work here

                IList<object> configurations = data["EventData"] as IList<object>;
                var configs = configurations[0] as ExpandoObject;
                EnvConfig = DeserializeConfiguration(configs);
                if (!string.IsNullOrEmpty(Validation(EnvConfig)))
                    return Validation(EnvConfig);
                DeserializeAndInitializeLogonEndPoint(EnvConfig);
                return "";

            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                return "Unable to get configurations from server.";
            }
        }

        public string Validation(EnvironmentConfig environmentConfig)
        {
            string ErrorMessage = string.Empty;
            if (EnvConfig.Endpoints == null)
                ErrorMessage = "Unable to get Endpoints from server\r";
            if (EnvConfig.ProtocolChannel == null)
                ErrorMessage += "Unable to get Protocol Channels from server\r";
            return ErrorMessage;
        }

        private static bool ProcessServerResponse(dynamic response, ref string stError)
        {
            if (response is null)
            {
                stError = "response is null: Request timed out.";
                return false;
            }

            //if (response.Status != 0)
            //{
            //    stError = response.Reason;
            //    return false;
            //}

            if (response.Data is null)
            {
                stError = "response.Data is null: Unable to get configurations from server.";
                return false;
            }

            if (response.Data.EventData is null)
            {
                stError = "response.Data.EventData is null: Unable to get configurations from server.";
                return false;
            }

            if (response.Data.EventData.Count < 1)
            {
                stError = "No Endpoints are configured on config server.";
                return false;
            }

            return true;
        }

        public void LoadAllEndPoints()
        {
            ProtocolChannelManager.Instance.ClearChannels();
            protocolChannelConfigurations.Clear();
            SerializeAndInitializeEndpoints(EnvConfig);
        }

        private void DeserializeAndInitializeLogonEndPoint(EnvironmentConfig environmentConfig)
        {
            var igniteEndpoint = environmentConfig.Endpoints.Where(x => x.Type == "Logon").FirstOrDefault();
            TextReader reader = new StringReader(igniteEndpoint.EndPoint_Data);
            EndPointConfiguration endpoint = (EndPointConfiguration)endpointsSerializer.Deserialize(reader);
            endpoint.Name = igniteEndpoint.Name;
            EndPointManager.Instance.Initialize(endpoint);
            var logonProvider = new LogonServiceProvider();
            logonProvider.Initialize(EndPointManager.Instance);
            GlobalProvider.Instance.AddProvider(logonProvider);
            LoadAllProtocolChannel(environmentConfig);

        }
        private void SerializeAndInitializeEndpoints(EnvironmentConfig environmentConfig)
        {
            try
            {
                foreach (ProtocolChannelIgniteConfiguration item in environmentConfig.ProtocolChannel)
                {
                    TextReader reader = new StringReader($"{item.ProtocolChannelData}");

                    protocolChannelConfigurations.Add((ProtocolChannelConfiguration)ProtocolSerializer.Deserialize(reader));
                }
                foreach (EndPointIgniteConfiguration item in environmentConfig.Endpoints)
                {
                    TextReader reader = new StringReader(item.EndPoint_Data);
                    EndPointConfiguration endpoint = (EndPointConfiguration)endpointsSerializer.Deserialize(reader);
                    endpoint.Name = item.Name;
                    endpointsConfigurations.Add(endpoint);
                }

                ProtocolChannelManager.Instance.Initialize(protocolChannelConfigurations);
                EndPointManager.Instance.Initialize(endpointsConfigurations.Where<EndPointConfiguration>(x => x.Name != "Logon").ToList());
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }

        }
        private void LoadAllProtocolChannel(EnvironmentConfig environmentConfig)
        {
            foreach (ProtocolChannelIgniteConfiguration item in environmentConfig.ProtocolChannel)
            {
                TextReader reader = new StringReader(item.ProtocolChannelData);

                protocolChannelConfigurations.Add((ProtocolChannelConfiguration)ProtocolSerializer.Deserialize(reader));
            }
            ProtocolChannelManager.Instance.Initialize(protocolChannelConfigurations);
        }

        private static EnvironmentConfig DeserializeConfiguration(ExpandoObject config)
        {
            return JsonConvert.DeserializeObject<EnvironmentConfig>(JsonConvert.SerializeObject(config));
        }
    }
}
