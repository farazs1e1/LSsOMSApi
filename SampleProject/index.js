const signalR = require("@microsoft/signalr");

// socket hub connection
const token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZFMDFERTY3RkY0MDNBRTI3RUVFQjNDNjcwNUY1N0YzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MjUwNDM1NDIsImV4cCI6MTYyNTA0NDQ0MiwiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1OTQ1NyIsImF1ZCI6WyJtYXJrZXRkYXRhLWFwaSIsIm9tcy1hcGkiXSwiY2xpZW50X2lkIjoiTFNMIiwiaW50ZXJuYWwiOiJ0cnVlIiwibWF4X2Nvbm5lY3Rpb25fYWxsb3dlZF9mb3JfaHViIjoiMiIsInN1YiI6ImFkZWVsIiwiYXV0aF90aW1lIjoxNjI1MDQzNTQyLCJpZHAiOiJsb2NhbCIsImp0aSI6IkE4NEUwODRDOUEzQjlCQTI0MzE5NTM4RTAzN0NGOTdCIiwiaWF0IjoxNjI1MDQzNTQyLCJzY29wZSI6WyJtYXJrZXRkYXRhLWFwaSIsIm9tcy1hcGkiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicGFzc3dvcmQiXX0.HBto-HtFbONQw8_i1fqPa-z87sWMukfmL3knUhOC8gOgd6kphMpv-YoVjf_AXoK5CRTBwIuVhhx4mywNL_dl_FYr8lu4zG1FoaMuS0qC9i9V61YhG6y4VwDRzM5O7t90c3Xh07AIgOP5parPlyOnPkpyz4WIn7YumLyF4wTdMQA7jvITmsmVqZF4TUj6FCTWO1TzzjCCxZI0kJdJNGJ72zWishaNCUey21yECthXI2ZvrzmIY4WstPcuWca4THSb82IuCMkjA1kc_MShuvHovjwVGb9Yu2KEHOUhMtUuiGTKabu7sHCb2Hj6wt265VNn1ou4IGSqqrakyVcZBgpsXQ";
let connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:63918/transactional", { accessTokenFactory: () => token })
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("Orders", (data) => {
    console.log("Orders", data);
});

connection.on("OpenOrders", (data) => {
    console.log("OpenOrders", data);
});

connection.on("Executions", (data) => {
    console.log("Executions", data);
});

connection.on("Positions", (data) => {
    console.log("Positions", data);
});

connection.on("NetLimitSummary", (data) => {
    console.log("NetLimitSummary", data);
});

connection.on("Locates", (data) => {
    console.log("Locates", data);
});

connection.on("LocateSummary", (data) => {
    console.log("LocateSummary", data);
});

start();

function start() {
    connection.start()
        .then(() => {
            console.log("Connected");
        });
};