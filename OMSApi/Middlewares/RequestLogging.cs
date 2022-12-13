using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using OMSServices.Implementation;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OMSApi.Middlewares
{
    public static class RequestLogging
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly bool _logRequestBody, _logResponseBody;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

        private static readonly int s_readChunkBufferLength = 4096;

        public RequestLoggingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _logRequestBody = bool.Parse(configuration["Logging:RequestBody"]);
            _logResponseBody = bool.Parse(configuration["Logging:ResponseBody"]);
        }

        public async Task Invoke(HttpContext httpContext, RequestInformation requestInformation, ILogger<RequestLoggingMiddleware> logger)
        {
            string dateTimeFormat = "dd/MM/yyyy HH:mm:ss:FFF";

            // try get client initiated request timestamp
            string clientInitiatedTimeStamp = null;
            if (httpContext.Request.Headers.TryGetValue("x-client-initiated-request-timestamp", out var value))
            {
                clientInitiatedTimeStamp = value;
            }

            // write log when request received
            logger.LogInformation("[Requested] {0}", new
            {
                requestInformation.RequestId,
                ClientInitiatedTimeStamp = clientInitiatedTimeStamp,
                ServerInitiatedRequestTimeStamp = requestInformation.ServerInitiatedRequestTimeStamp.ToString(dateTimeFormat)
            });

            if (_logRequestBody)
            {
                await ReadRequestBodyInChunksAsync(httpContext, logger);
            }

            // send timestamp headers
            httpContext.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                requestInformation.ServerRespondTimeStamp = DateTime.UtcNow;
                httpContext.Response.Headers.Add("x-request-id", requestInformation.RequestId);
                if (!string.IsNullOrWhiteSpace(clientInitiatedTimeStamp))
                {
                    httpContext.Response.Headers.Add("x-client-initiated-request-timestamp", clientInitiatedTimeStamp);
                }
                httpContext.Response.Headers.Add("x-server-initiated-request-timestamp", requestInformation.ServerInitiatedRequestTimeStamp.ToString(dateTimeFormat));
                httpContext.Response.Headers.Add("x-server-responsed-timestamp", requestInformation.ServerRespondTimeStamp.ToString(dateTimeFormat));
                //httpContext.Response.Headers.Add("x-taken-times", string.Join(",", requestInformation.TakenTimes.Select(x => new { Desc = x.Key, Time = $"{x.Value}ms" }.ToString())));
                return Task.CompletedTask;
            }, httpContext);

            // log respond request
            httpContext.Response.OnCompleted(async state =>
            {
                var httpContext = (HttpContext)state;
                await Task.Run(() =>
                {
                    logger.LogInformation("[Respond] {0}", new
                    {
                        requestInformation.RequestId,
                        ClientInitiatedTimeStamp = clientInitiatedTimeStamp,
                        ServerInitiatedRequestTimeStamp = requestInformation.ServerInitiatedRequestTimeStamp.ToString(dateTimeFormat),
                        ServerRespondTimeStamp = requestInformation.ServerRespondTimeStamp.ToString(dateTimeFormat)/*,
                            TakenTimes = string.Join(",", requestInformation.TakenTimes.Select(x => new { Desc = x.Key, Time = $"{x.Value}ms" }.ToString()))*/
                    });
                });
            }, httpContext);

            if (_logResponseBody)
            {
                await CallNextAndLogResponseBodyAsync(httpContext, logger);
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task ReadRequestBodyInChunksAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
        {
            // source: https://stackoverflow.com/a/52328142
            // source: https://stackoverflow.com/a/64927281

            context.Request.EnableBuffering();

            await using MemoryStream stream = _recyclableMemoryStreamManager.GetStream();

            await context.Request.Body.CopyToAsync(stream);

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[s_readChunkBufferLength];

            int readChunkLength;
            //do while: is useful for the last iteration in case readChunkLength < chunkLength
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, s_readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            string result = textWriter.ToString();

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation("Serialized request body: {0}", result);
        }

        private async Task CallNextAndLogResponseBodyAsync(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
        {
            await using MemoryStream memoryStream = _recyclableMemoryStreamManager.GetStream();

            Stream originalBodyStream = context.Response.Body;
            context.Response.Body = memoryStream;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseBodySerialized = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation("Serialized response body: {0}", responseBodySerialized);

            await memoryStream.CopyToAsync(originalBodyStream);
        }

        private static async Task<string> ReadResponseBodyAsync(HttpResponse httpResponse)
        {
            byte[] buffer = new byte[(int)httpResponse.ContentLength];
            await httpResponse.Body.ReadAsync(buffer, 0, buffer.Length);
            string result = System.Text.Encoding.ASCII.GetString(buffer);

            //httpResponse.EnableBuffering();
            //string result;
            //using (var streamReader = new StreamReader(httpResponse.Body))
            //{
            //    result = await streamReader.ReadToEndAsync();
            //}
            //httpResponse.Body.Position = 0;
            return result;
        }

        private static async Task ReadRequestBodyAsync(HttpRequest httpRequest, ILogger<RequestLoggingMiddleware> logger)
        {
            //byte[] buffer = new byte[(int)httpContext.Request.ContentLength];
            //await httpContext.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            //requestBody = System.Text.Encoding.ASCII.GetString(buffer);

            // sources:
            // https://stackoverflow.com/a/40994711
            // https://stackoverflow.com/a/58089658
            httpRequest.EnableBuffering();

            //await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            //await httpRequest.Body.CopyToAsync(requestStream);
            //await httpRequest.Body.CopyToAsync(requestStream);

            string result;
            // https://stackoverflow.com/a/60135776
            using (var streamReader = new StreamReader(httpRequest.Body, leaveOpen: true))
            {
                result = await streamReader.ReadToEndAsync();
            }
            httpRequest.Body.Position = 0;

            logger.LogInformation("Serialized request body: {0}", result);
        }
    }
}
