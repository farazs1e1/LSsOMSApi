from datetime import date
from app import tokens
#headers to be send along with request 

#header for login 
headers_login= {'Content-type':'application/x-www-form-urlencoded',"x-client-request-timestamp":"%s"%date.today()}

#header for getting static data
headers_staticData= {'Authorization': "Bearer %s"%tokens.token_get(),"x-client-request-timestamp":"%s"%date.today()}

#header for getting order
headers_Order = {"x-client-request-timestamp":"%s"%date.today(),"Content-Type":"application/json",'Authorization':"Bearer %s"%tokens.token_get()}

#headers for Subscribing 
headers_Subscribe ={"x-client-initiated-request-timestamp ":"%s"%date.today(),'Authorization': "Bearer %s"%tokens.token_get()}
headers_positionSubscribe={"x-client-initiated-request-timestamp ":"%s"%date.today(),'Authorization': "Bearer %s"%tokens.token_get()}
headers_positionUnSubscribe={"x-client-initiated-request-timestamp ":"%s"%date.today(),'Authorization': "Bearer %s"%tokens.token_get()}
