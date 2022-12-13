namespace OMSServices.Utils
{
    public static class BytesConversion
    {
        public static Type FromBytes<Type>(this byte[] bytes)
        {
            if (bytes == null)
                return default;

            return Utf8Json.JsonSerializer.Deserialize<Type>(bytes);
        }

        public static byte[] ToBytes<Type>(this Type value)
        {
            return Utf8Json.JsonSerializer.Serialize(value);
        }
    }
}
