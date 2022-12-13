using Microsoft.Extensions.Logging;
using OMSServices.Enum;

namespace OMSServices.Utils
{
    public static class EnumsHelper
    {
        private static readonly ILogger s_logger = StaticLogger.CreateInstance("EnumsHelper");

        public static T ParseEnum<T>(string value)
        {
            if (value == null)
            {
                return default;
            }
            return (T)System.Enum.Parse(typeof(T), value, true);
        }

        public static OpenClose ConvertToOpenClose(object value)
        {
            if (value is double valueDouble)
            {
                return (OpenClose)valueDouble;
            }
            else if (value is null)
            {
                return 0;
            }
            else if (value is string valueStr)
            {
                if (valueStr.Length == 0)
                {
                    return 0;
                }
                else if (valueStr.Length == 1)
                {
                    return (OpenClose)valueStr[0];
                }
            }
            s_logger.LogWarning("Encountered unhandled OpenClose value: '{value}', type: {valueType}", value, value.GetType());
            return 0;
        }
    }
}
