namespace OMSServices.Utils
{
    public static class JsonConverter
    {
        public static string SerializeObject(object obj)
        {
            return Utf8Json.JsonSerializer.ToJsonString(obj);
        }

        public static T DeserializeObject<T>(string value)
        {
            return Utf8Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}
