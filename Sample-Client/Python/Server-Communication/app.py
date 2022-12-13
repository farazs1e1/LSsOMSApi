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
            results = requests.post(api_urls.token,data.data_client_credentials,headers=headers.headers_login).json()
            access_token = results['access_token']
            return access_token
    
# class to Get the Static Data
class staticData:
    def staticData(type,userdesc):
        from headers import headers
        Static_Data=requests.get(api_urls.static_data+type,headers=headers.headers_staticData,params={"userDesc":"%s"%userdesc})
        return Static_Data


class OrderService:
    def AddOrder(Data,userdesc):
        from headers import headers
        results=requests.post(api_urls.Orders,headers=headers.headers_Order, data=json.dumps(Data),params={"userDesc":"%s"%userdesc} )   
        return results

    def UpdateOrder(Data,userdesc):
        from headers import headers
        results=requests.put(api_urls.Orders,headers=headers.headers_Order, data=json.dumps(Data),params={"userDesc":"%s"%userdesc} )
        return results

    def CancelOrder(Data):
        from headers import headers
        results=requests.delete(api_urls.Orders+"/%s"%Data["qOrderID"]+"/%s"%Data["userDesc"],headers=headers.headers_Order)
        return results
#class to subscribe to orders updates after that we can recieve updates from sockets
class Orders:
    def subscribe_Orders(userdesc):
        from headers import headers
        results=requests.get(api_urls.Orders_Subscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc})
        return results

    def unsubscribe_Orders(userdesc):
        from headers import headers
        results=requests.get(api_urls.Orders_UnSubscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc}) 
        return results
#class to subscribe to Openorders updates after that we can recieve updates from sockets
class Openorders:   
    def subscribe_openorders(userdesc):
        from headers import headers
        results=requests.get(api_urls.openorders_subscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc})
        return results

    def unsubscribe_openorders(userdesc):
        from headers import headers
        results=requests.get(api_urls.openorders_unsubscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc})
        return results

#class to subccribe to Executions updates after that we can recieve updates from sockets    
class Executions:   
    def subscribe_executions(userdesc):
        from headers import headers
        results=requests.get(api_urls.executions_Subscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc})
        
        return results
    def unsubscribe_executions(userdesc):
        from headers import headers
        results=requests.get(api_urls.executions_UnSubscribe,headers=headers.headers_Subscribe,params={"userDesc":"%s"%userdesc})
        return results

#class to subccribe to Positions updates after that we can recieve updates from sockets
class Positions:
    def pos_subs(userdesc):
        from headers import headers
        results = requests.get(api_urls.Position_subscribe,headers=headers.headers_positionSubscribe,params={"userDesc":"%s"%userdesc})
        return results
    
    def pos_unsubs(userdesc):
        from headers import headers
        results = requests.get(api_urls.Position_unsubscribe,headers=headers.headers_positionUnSubscribe,params={"userDesc":"%s"%userdesc})
        return results
#sockets sub class is for getting live updates from the subscribed topics using websockets
class SocketsSubs:
   async def subs_orders_listen():      
       async with websockets.connect(api_urls.SocketSubscription_url+tokens.token_get()) as ws:
           while (True):
               msg = await ws.recv()
               print(msg)
    
   