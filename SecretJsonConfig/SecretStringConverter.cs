using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecretJsonConfig
{
    public class SecretStringConverter : JsonConverter<String>
    {
        public override String Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var s = reader.GetString();
            return s == null ? s : s[1..^1];
        }

        public override void Write(
            Utf8JsonWriter writer,
            String stringValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(stringValue == null ? stringValue : $"*{stringValue}*");
    }
}
