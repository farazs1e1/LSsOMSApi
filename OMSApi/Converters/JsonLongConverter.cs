using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OMSApi.Converters
{
    public class JsonLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return reader.GetInt64();
            }
            catch
            {
                return 0;
            }
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
