using Microsoft.Extensions.Logging;

namespace OMSServices.Utils
{
    /// <summary>
    /// It is not intended to be used everywhere, use the Microsoft.Extensions.Logging.ILogger by dependency injection wherever you can.
    /// It is only meant for pure static classes where dependency injection is barely possible.
    /// Have kept this logger's implementation in a separate static-class so that in future when we need to implement any other logger we change the implementaion in this class and Program.cs only.
    /// </summary>
    public class StaticLogger
    {
        public static ILogger CreateInstance(string categoryName)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddLog4Net();

            return loggerFactory.CreateLogger(categoryName);
        }
    }
}
