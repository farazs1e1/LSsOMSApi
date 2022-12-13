import json
import urllib
import requests
from urls import api_urls
from data import data
import websockets 

# class to get the token from the given base Url
class tokens:
        def token_get():
            from headers import headers
            results = requests.post(api_urls.token,data.data,headers=headers.headers_login).json()
            access_token = results['access_token']
            return access_token
    
# class to Get the Static Data
class staticData:
    def staticData(inp):
        from headers import headers
        Static_Data=requests.get(api_urls.static_data+inp,headers=headers.headers_staticData)
        return Static_Data


class OrderService:
    def AddOrder(Data):
        from headers import headers
        results=requests.post(api_urls.Orders,headers=headers.headers_Order, data=json.dumps(Data) )
        return results
# Sr. no. Field Required DataType Description
# 1     OrderQty  Yes      Long   Updated order quantity value
# 2      Price    Yes    Decimal  Updated price value
# 3    QOrderID   Yes     Long    Valid order id 
    def UpdateOrder(Data):
        from headers import headers
        results=requests.put(api_urls.Orders,headers=headers.headers_Order, data=json.dumps(Data))
        return results

    def CancelOrder(Data):
        from headers import headers
        results=requests.delete(api_urls.Orders+"/%s"%Data["qOrderID"],headers=headers.headers_Order)
        return results
#class to subscribe to orders updates after that we can recieve updates from sockets
class Orders:
    def subscribe_Orders():
        from headers import headers
        results=requests.get(api_urls.Orders_Subscribe,headers=headers.headers_Subscribe)
        return results

    def unsubscribe_Orders():
        from headers import headers
        results=requests.get(api_urls.Orders_UnSubscribe,headers=headers.headers_Subscribe)
        return results
#class to subscribe to Locates updates after that we can recieve updates from sockets
class Locates:   
    def subscribe_locates():
        from headers import headers
        results=requests.get(api_urls.Locates_Subscribe,headers=headers.headers_Subscribe)
        return results
    def unsubscribe_locates():
        from headers import headers
        results=requests.get(api_urls.Locates_UnSubscribe,headers=headers.headers_Subscribe)
        return results

#class to LocatesSummary to Locates updates after that we can recieve updates from sockets    
class LocatesSummary:   
    def subscribe_locatesSummary():
        from headers import headers
        results=requests.get(api_urls.Locates_Subscribe,headers=headers.headers_Subscribe)
        return results
    def unsubscribe_locatesSummary():
        from headers import headers
        results=requests.get(api_urls.Locates_UnSubscribe,headers=headers.headers_Subscribe)
        return results
#class to AccountBalances to Locates updates after that we can recieve updates from sockets
class AccountBalances:
    def subscribe_AccountBalances():
        from headers import headers
        results=requests.get(api_urls.AccountBalances_Subscribe,headers=headers.headers_Subscribe)
        return results

    def unsubscribe_AccountBalances():
        from headers import headers
        results=requests.get(api_urls.AccountBalances_UnSubscribe,headers=headers.headers_Subscribe)
        return results
#class to Positions to Locates updates after that we can recieve updates from sockets
class Positions:
    def pos_subs():
        from headers import headers
        results = requests.get(api_urls.Position_subscribe,headers=headers.headers_positionSubscribe)
        return results
    
    def pos_unsubs():
        from headers import headers
        results = requests.get(api_urls.Position_unsubscribe,headers=headers.headers_positionUnSubscribe)
        return results
#sockets sub class is for getting live updates from the subscribed topics using websockets
class SocketsSubs:
   async def subs_orders_listen():      
       async with websockets.connect(api_urls.SocketSubscription_url+tokens.token_get()) as ws:
           while (True):
               msg = await ws.recv()
               print(msg)
    
   