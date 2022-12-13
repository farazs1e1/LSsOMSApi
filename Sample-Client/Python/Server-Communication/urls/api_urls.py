from urls.baseUrl import base_Url,scope,socket_baseURl
#urls defined for usage for getting token and other functionalities 
token = base_Url+"connect/token"
static_data = base_Url+scope+"ord/sc/api/staticData/"
Orders = base_Url+scope+"ord/sc/api/orders"
Orders_Subscribe =base_Url +scope+"ord/sc/api/subscription/orders/subscribe"
Orders_UnSubscribe = base_Url+scope+"ord/sc/api/subscription/orders/unsubscribe"
Position_subscribe = base_Url+scope+"ord/sc/api/subscription/positions/subscribe"
Position_unsubscribe = base_Url+scope+"ord/sc/api/subscription/positions/unsubscribe"
openorders_subscribe=base_Url+scope+"ord/api/subscription/openorders/subscribe"
openorders_unsubscribe=base_Url+scope+"ord/api/subscription/openorders/unsubscribe"
executions_Subscribe=base_Url+scope+"ord/api/subscription/executions/subscribe"
executions_UnSubscribe=base_Url+scope+"ord/api/subscription/executions/unsubscribe"

#url for sockets communication
SocketSubscription_url=socket_baseURl+"/transactionalws?access_token="
