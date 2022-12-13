using Microsoft.Extensions.Logging;
using System.Dynamic;

namespace OMSServices.Utils
{
    public static class TypeConversion
    {
        private static readonly ILogger _logger = StaticLogger.CreateInstance("TypeConversion");

        public static T ConvertExpandoObjectTo<T>(this ExpandoObject expandoObject) where T : class
        {
            string serialized = Utf8Json.JsonSerializer.ToJsonString(expandoObject);
            try
            {
                return Utf8Json.JsonSerializer.Deserialize<T>(serialized);
            }
            catch (Utf8Json.JsonParsingException ex)
            {
                _logger.LogError(ex, message: ex.Message);
                _logger.LogWarning(serialized);
                return null;
            }
        }
    }
}
