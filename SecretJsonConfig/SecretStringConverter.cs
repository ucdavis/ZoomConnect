using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecretJsonConfig
{
    public class SecretStringConverter : JsonConverter<string>
    {
        public override string Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            var s = reader.GetString();
            if (s == null) { return ""; }

            return s[1..^1];
        }

        public override void Write(
            Utf8JsonWriter writer,
            string secretValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(secretValue == null ? "" : $"*{secretValue}*");
    }
}
