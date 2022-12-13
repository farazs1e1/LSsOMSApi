using OMSServices.Models;
using OMSServices.Services;
using OMSServices.Data;
using Client.Providers;
using System.Collections.Generic;
using Client.Providers.Utilities;
using System.Dynamic;
using System.Linq;
using OMSServices.Enum;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace OMSServices.Implementation
{
    class OrderManagementService : IOrderManagementService
    {
        private const string _orderExecutionProviderName = "OrderExec";
        private readonly RequestInformation requestInformation;
        private readonly IConfiguration configuration;
        private readonly IStaticDataService staticDataService;

        public OrderManagementService(RequestInformation requestInformation,
            IConfiguration configuration,
            IStaticDataService staticDataService)
        {
            this.requestInformation = requestInformation;
            this.configuration = configuration;
            this.staticDataService = staticDataService;
        }

        public ResultDto CancelOrderV3(long qOrderId, string boothId, string originatingUserDesc)
        {
            var result = CancelOrderAsync(qOrderId, boothId, originatingUserDesc) as string;
            if (string.IsNullOrEmpty(result))
            {
                return new ResultDto(true, "Order cancelled successfully.");
            }
            return new ResultDto(false, result);
        }

        public object CancelOrderAsync(long QOrderID, string boothId, string originatingUserDesc)
        {
            IDictionary<string, object> currOrder = new ExpandoObject() as IDictionary<string, object>;
            var provider = requestInformation.WatchRequestTime("Get service provider and create object for cancel order", () =>
            {
                currOrder.Add("QOrderID", QOrderID);
                currOrder.Add("TargetLocationID", boothId);
                currOrder.Add("OriginatingUserDesc", originatingUserDesc);
                MessageProtocol.GetHeaderTags(ref currOrder, MsgType.ORDERCANCELREQUEST);

                return GlobalProvider.Instance.GetProvider<Client.Providers.Interfaces.IServiceProvider>(_orderExecutionProviderName);
            });
            return requestInformation.WatchRequestTime("Send data to service provider for cancel order", () => provider.SendData(DataHelper.WrapBOEMessage(currOrder, boothId)));
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(BindableOEMessage order, string userIdentifier)
        {
            order = await AddClientIdInBindableOEMessageAsync(order, userIdentifier);
            if (order == null)
                return new CreateOrderResponse { Message = "Sorry, this user has no associated account." };

            if (order.Destination == null)
            {
                var destination = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Destination, order.OriginatingUserDesc, order.BoothID, userIdentifier);
                var defaultDestination = destination.EventData.FirstOrDefault(x => x.IsDefault);
                order.Destination = defaultDestination?.Value ?? configuration["DefaultOrderDestination"];
            }
            return requestInformation.WatchRequestTime("Send data to service provider for create order", () =>
            {
                var result = Helper.SendDatatoServiceProvider(_orderExecutionProviderName, DataHelper.WrapBOEMessage(order.ToDictionary(), order.BoothID));
                if (string.IsNullOrWhiteSpace(result.ToString()))
                {
                    return new CreateOrderResponse
                    {
                        Success = true,
                        Message = "Order successfully created."
                    };
                }
                else
                {
                    ServerResponse response = Validator.ProcessServerResponse(result.ToString());

                    if (response.ReplyType == ENServerReplyType.SERVER_REPLY_NONE
                        || response.ReplyType == ENServerReplyType.SERVER_REPLY_ERROR)
                    {
                        return new CreateOrderResponse
                        {
                            Message = response.Message
                        };
                    }
                    else
                    {
                        return new CreateOrderResponse
                        {
                            Confirmation = true,
                            ServerResponse = response
                        };
                    }
                }
            });
        }

        public Task<object> UpdateOrderAsync(BindableOEMessage order, string userIdentifier)
        {
            return requestInformation.WatchRequestTime("Send data to service provider for update order", () =>
            {
                //have to find proper solution for this modify case only
                var currOrder = order.ToDictionary();
                if (order.Price <= 0)
                {
                    currOrder.Remove("Price");
                }

                object response = Helper.SendDatatoServiceProvider(_orderExecutionProviderName, DataHelper.WrapBOEMessage(currOrder, order.BoothID));
                return Task.FromResult(response);
            });
        }

        private async Task<BindableOEMessage> AddClientIdInBindableOEMessageAsync(BindableOEMessage order, string userIdentifier)
        {
            if (order.Account == null || order.ClientID == null)
            {
                var accounts = await staticDataService.GetStaticDataAsync<StaticDataValues>(QueryType.Account, order.OriginatingUserDesc, order.BoothID, userIdentifier);

                //If both are null then set default account with default client id
                if (order.Account == null)
                {
                    var defaultAccount = accounts.EventData.FirstOrDefault(x => x.IsDefault);
                    if (defaultAccount == null)
                        return null;
                    order.Account = defaultAccount.Value;
                    order.ClientID = defaultAccount.Name;
                }
                //Else if account is present then set client id based on that account
                else
                {
                    var reqAccount = accounts.EventData.FirstOrDefault(x => x.Value == order.Account);
                    //If no respective account is found against the set order.account then copy it as it is.
                    order.ClientID = reqAccount?.Name ?? order.Account;
                }
            }
            return order;
        }
    }
}
