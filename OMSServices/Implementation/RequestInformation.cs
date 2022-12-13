using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OMSServices.Implementation
{
    public class RequestInformation
    {
        public RequestInformation()
        {
            RequestId = Guid.NewGuid().ToString();
            ServerInitiatedRequestTimeStamp = DateTime.UtcNow;
            TakenTimes = new Dictionary<string, long>();
        }

        public string RequestId { get; }
        public DateTime ServerInitiatedRequestTimeStamp { get; }
        public DateTime ServerRespondTimeStamp { get; set; }
        public Dictionary<string, long> TakenTimes { get; }

        public void WatchRequestTime(string key, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action.Invoke();
            stopwatch.Stop();
            TakenTimes.TryAdd(key, stopwatch.ElapsedMilliseconds);
        }

        public async Task WatchRequestTimeAsync(string key, Func<Task> func)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await func.Invoke();
            stopwatch.Stop();
            TakenTimes.TryAdd(key, stopwatch.ElapsedMilliseconds);
        }

        public T WatchRequestTime<T>(string key, Func<T> func)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = func.Invoke();
            stopwatch.Stop();
            TakenTimes.TryAdd(key, stopwatch.ElapsedMilliseconds);
            return result;
        }

        public async Task<T> WatchRequestTimeAsync<T>(string key, Func<Task<T>> func)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await func.Invoke();
            stopwatch.Stop();
            TakenTimes.TryAdd(key, stopwatch.ElapsedMilliseconds);
            return result;
        }
    }
}
