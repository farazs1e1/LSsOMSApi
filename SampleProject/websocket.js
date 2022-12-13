const WebSocket = require('ws');
var accessToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjZFMDFERTY3RkY0MDNBRTI3RUVFQjNDNjcwNUY1N0YzIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2MjU3NTM5NjMsImV4cCI6MTYyNTc1NDg2MywiaXNzIjoiaHR0cDovL2xvY2FsaG9zdDo1OTQ1NyIsImF1ZCI6WyJtYXJrZXRkYXRhLWFwaSIsIm9tcy1hcGkiXSwiY2xpZW50X2lkIjoiTFNMIiwiaW50ZXJuYWwiOiJ0cnVlIiwibWF4X2Nvbm5lY3Rpb25fYWxsb3dlZF9mb3JfaHViIjoiMiIsInN1YiI6ImFkZWVsIiwiYXV0aF90aW1lIjoxNjI1NzUzOTYzLCJpZHAiOiJsb2NhbCIsImp0aSI6IkU4REQ3RTg0QUY1QjgwRTNDNUY0ODIxMDREOUNGMUI2IiwiaWF0IjoxNjI1NzUzOTYzLCJzY29wZSI6WyJtYXJrZXRkYXRhLWFwaSIsIm9tcy1hcGkiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicGFzc3dvcmQiXX0.fC2e6O3rqNJUDDsyqbGi3Jdo4a3NNG9gmZ3EK_-L9ZK68jZGK7gygM3Jx7y8KjdfM8EFWfeVtKMNw1TTfM9E0YrqeWDYxbyBVVdftnrfPghHTFgw05N44gtw8gt7LKonySaTLuZ5-Bi9G2jzNHcOFxqJF01PG-2CAaHsNAVvkToPJZKNKAeCSU6rfwrzOots1-qB1iENbSrZ22s6MpKHnX7j730440vI7EHsvx9tjmNAtmLo7oKbNEweXcdlfUy-D8TayuD09terg8OfB9uciBMnDPYsLFvo_LERfG320ZtJwBSOF5L46_phLtGzdpUnMUAh8k6hWz-ZoqBe9XXoZw";
// var websocket = new WebSocket("ws://localhost:63918/transactionalws?access_token=" + accessToken);
var websocket = new WebSocket("ws://localhost:63918/transactionalws?access_token=" + accessToken);

websocket.onopen = function (e) {
    console.log("Connected");
};

websocket.onclose = function (e) {
    console.log("Closed");
};

websocket.onmessage = function (e) {
    console.log(e.data);
};

websocket.onerror = function (e) {
    console.log("Error", e);
};