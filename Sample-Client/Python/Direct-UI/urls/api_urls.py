from urls.baseUrl import base_Url, scope,socket_baseURl

#urls defined for usage for getting token and other functionalities

token = base_Url +"connect/token"

static_data = base_Url +scope +"ord/api/staticData/"

Orders = base_Url +scope+"ord/api/orders"

Orders_Subscribe =base_Url  +scope +"ord/api/subscription/orders/subscribe"

Orders_UnSubscribe = base_Url +scope +"ord/api/subscription/orders/unsubscribe"

Locates_Subscribe =base_Url +scope +"ord/api/subscription/locates/subscribe"

Locates_UnSubscribe = base_Url + scope+"ord/api/subscription/locates/unsubscribe"

LocatesSummary_Subscribe = base_Url +scope +"ord/api/subscription/locates/summary/subscribe"

LocatesSummary_UnSubscribe = base_Url +scope +"ord/api/subscription/locates/summary/unsubscribe"

AccountBalances_Subscribe = base_Url + scope+"ord/api/subscription/locates/balances/subscribe"

AccountBalances_UnSubscribe = base_Url + scope+"ord/api/subscription/locates/balances/unsubscribe"

Position_subscribe = base_Url +scope +"ord/api/subscription/positions/subscribe"

Position_unsubscribe = base_Url +scope +"ord/api/subscription/positions/unsubscribe"

#url for sockets communication

SocketSubscription_url=socket_baseURl +"/transactionalws?access_token="
