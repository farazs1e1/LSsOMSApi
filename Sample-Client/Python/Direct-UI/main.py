#caller Code 
#here we are using our sample python implementation 
from app import *
import asyncio
#getting acces token 
token = tokens.token_get()  
print(token)  
#getting static Data
staticData_Side=staticData.staticData("Side")  
staticData_Account =staticData.staticData("Account")  
staticData_TIF =staticData.staticData("TIF")  
staticData_OrdType =staticData.staticData("OrdType")
staticData_LocateTIF =staticData.staticData("LocateTIF")

print("Sides")
print(staticData_Side.text)
print("Account")
print(staticData_Account.text)
print("Time In Force")
print(staticData_TIF.text)
print("Order Type")
print(staticData_OrdType.text)
print("Locate time in force")
print(staticData_LocateTIF.text)

dict = {
    "ordType": "2",
    "side": "1",
    "symbol": "MSFT",
    "timeInForce": "0",
    "account": "stresstestaccount_1",
    "orderQty": 8000,
    "price":200
}
dict_update = {
    "price":100,
    "OrderQty": 2000,
    "QOrderID": 8,
    "OrdType": "2",    
}
dict_cncl = {    
    "qOrderID": 36,   
}
#subscribing to topics
order_subs=Orders.subscribe_Orders()  
position_subs = Positions.pos_subs()  
 
print("Subscribe Order"+order_subs.text) 
print(position_subs.text) 
# similarly you can unsubscribe
order_unsubs=Orders.unsubscribe_Orders() 
position_unsubs = Positions.pos_unsubs()  

# order Creattion/Updation/cancellation
createOrder  = OrderService.AddOrder(dict)
print(createOrder.text)
UpdateOrder = OrderService.UpdateOrder(dict_update)
print(UpdateOrder.text)
cancelOrder = OrderService.CancelOrder(dict_cncl)
print(cancelOrder.text)

#Socket Call to get updates
asyncio.get_event_loop().run_until_complete(SocketsSubs.subs_orders_listen())