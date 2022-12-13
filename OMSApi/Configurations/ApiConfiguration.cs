using Client.Communication;
using Client.Communication.Configurations;
using Client.Communication.EndPoints;
using Client.Communication.ProtocolChannel;
using Client.Providers;
using Client.Providers.ServiceProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OMSServices.Utils;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq.Dynamic.Core;
using System.Xml.Serialization;

namespace OMSApi.Configurations
{
    public class ApiConfiguration
    {
        private static readonly ILogger logger = StaticLogger.CreateInstance("ApiConfiguration");

        public static void PreStartup(IConfiguration configuration)
        {
            ParsingConfig.Default.CustomTypeProvider = new MyCustomTypeProvider(); // Registration of MyCustomTypeProvider, to parse Lambda Expressions. (NECESSARY STEP BEFORE EVERYTHING)

            GetConfigsFromServerAndInitializeEndPoints(configuration);
            //InitializeEndPoints();
        }

        /// <summary>
        /// New implementation with Config Server. Sends login request by using this API's credentials from appsettings.
        /// </summary>
        /// <param name="configuration"></param>
        /// <exception cref="Exception"></exception>
        private static void GetConfigsFromServerAndInitializeEndPoints(IConfiguration configuration)
        {
            logger.LogInformation("OMS API version is: {apiVersion}", AssemblyHelper.GetEntryAssemblyVersion());

            var queryEndPointConfiguration = new QueryEndPointConfiguration()
            {
                ProtocolChannel = JsonChannel.Instance,
                QueryEndPoint = configuration["ConfigServer:QueryEndpoint"],
                QueyTimeout = TimeSpan.FromSeconds(int.Parse(configuration["ConfigServer:QueryTimeoutSeconds"])),
            };

            logger.LogInformation("Initializing ConfigEndPoint.");
            var queryEndpoint = new QueryEndPoint<string, ExpandoObject, ExpandoObject>(queryEndPointConfiguration);
            queryEndpoint.Run();
            EndPointManager.Instance.AddEndPoint("Config", queryEndpoint);

            logger.LogInformation("Initializing ConfigServiceProvider.");
            var configServiceProvider = new ConfigServiceProvider();
            configServiceProvider.Initialize(EndPointManager.Instance);
            GlobalProvider.Instance.AddProvider(configServiceProvider);

            var requestBody = new ExpandoObject();

            requestBody.TryAdd("RequestType", "HardLogin");
            requestBody.TryAdd("UserName", configuration["ConfigServer:Username"]);
            requestBody.TryAdd("BoothID", configuration["ConfigServer:BoothId"]);
            requestBody.TryAdd("Password", configuration["ConfigServer:Password"]);
            requestBody.TryAdd("AppVersion", configuration["ConfigServer:AppVersion"]);
            requestBody.TryAdd("ComputerIdentifier", configuration["ConfigServer:ComputerIdentifier"]);
            //requestBody["PublicIp"] = PublicIp;
            requestBody.TryAdd("TimeStamp", DateTime.UtcNow);

            logger.LogInformation("Fetching environment info from Config/Environment Server @ {QueryEndPoint}", queryEndPointConfiguration.QueryEndPoint);

            string response = EnvironmentManager.Instance.GetEnvironmentAndLoadLogon(requestBody);
            if (!string.IsNullOrEmpty(response))
            {
                string msg = "Failed to fetch environment info. Reason: " + response;
                logger.LogError(msg);
                throw new Exception(msg);
            }

            logger.LogInformation("Initializing EndPoints.");
            EnvironmentManager.Instance.LoadAllEndPoints();

            logger.LogInformation("Initializing Providers.");
            GlobalProvider.Instance.LoadAllProviders();
        }

        /// <summary>
        /// Old implementation. Used to initialize endpoints like below before multi-vTrader implementation.
        /// </summary>
        private static void InitializeEndPoints()
        {
            CommunicationConfiguration communicationConfiguration;
            var xmlSerializer = new XmlSerializer(typeof(CommunicationConfiguration));
            using (var configFile = new FileStream("CommunicationConfiguration.xml", FileMode.Open))
            {
                communicationConfiguration = xmlSerializer.Deserialize(configFile) as CommunicationConfiguration;
            }
            ProtocolChannelManager.Instance.Initialize(communicationConfiguration.ProtocolChannels);
            EndPointManager.Instance.Initialize(communicationConfiguration.EndPoints);
        }
    }
}
