#caller Code 
#here we are using our sample python implementation 
from app import *
import asyncio

#getting acces token  
token = tokens.token_get()  
print(token)  
#getting static Data
staticData_Side=staticData.staticData("Side","RegTrBug")  
staticData_Account =staticData.staticData("Account","RegTrBug")  
staticData_TIF =staticData.staticData("TIF","RegTrBug")  
staticData_OrdType =staticData.staticData("OrdType","RegTrBug")
staticData_LocateTIF =staticData.staticData("LocateTIF","RegTrBug")
staticData_Destination =staticData.staticData("Destination","RegTrBug")
staticData_CommType =staticData.staticData("CommType","RegTrBug")
staticData_MktTopPerfCateg =staticData.staticData("MktTopPerfCateg","RegTrBug")

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
print("Destination")
print (staticData_Destination.text)
print("CommType")
print (staticData_CommType.text)
print("MktTopPerfCateg")
print (staticData_MktTopPerfCateg.text)

dict = {
    "ordType": "2",
    "side": "1",
    "symbol": "GOOG",
    "timeInForce": "0",
    "account": "10001",
    "destination":"ARCA",
    "orderQty": 800000,
    "price":2000
}
dict_update = {
    "price":100,
    "OrderQty": 2000,
    "QOrderID": 40,
    "OrdType": "2"  
}
dict_cncl = {    

    "qOrderID": 40,  "userDesc":"RegTrBug"   
}
# #subscribing to topics
order_subs=Orders.subscribe_Orders("RegTrBug")  
position_subs = Positions.pos_subs("RegTrBug")  
openorder_subs= Openorders.subscribe_openorders("RegTrBug")
execution_subs = Executions.subscribe_executions("RegTrBug")
print("Subscribing Order"+order_subs.text) 
print("Subscribing Positions"+position_subs.text)
print("Subscribing Openorders"+openorder_subs.text)
print("Subscribing Executions"+execution_subs.text) 
# similarly you can unsubscribe
  
order_unsubs=Orders.unsubscribe_Orders("RegTrBug") 
position_unsubs = Positions.pos_unsubs("RegTrBug")  
openorder_unsubs= Openorders.unsubscribe_openorders("RegTrBug")
execution_unsubs = Executions.unsubscribe_executions("RegTrBug")
# # order Creation/Updation/cancellation
createOrder  = OrderService.AddOrder(dict,"RegTrBug")
print(createOrder.text)
UpdateOrder = OrderService.UpdateOrder(dict_update,"RegTrBug")
print(UpdateOrder.text)
cancelOrder = OrderService.CancelOrder(dict_cncl)
print(cancelOrder.text)

# #Socket Call to get updates
asyncio.get_event_loop().run_until_complete(SocketsSubs.subs_orders_listen())