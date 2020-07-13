using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SecretJsonConfig
{
    public class SecretStructConverter : JsonConverter<SecureStruct>
    {
        private readonly JsonEncodedText ValueName = JsonEncodedText.Encode("Value");

        private readonly JsonConverter<string> _stringConverter;

        public SecretStructConverter(JsonSerializerOptions options)
        {
            if (options?.GetConverter(typeof(string)) is JsonConverter<string> stringConverter)
            {
                _stringConverter = stringConverter;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override SecureStruct Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            };

            string value = null;
            bool valueSet = false;

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.ValueTextEquals(ValueName.EncodedUtf8Bytes))
            {
                value = ReadProperty(ref reader, options);
                valueSet = true;
            }
            else
            {
                throw new JsonException();
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new SecureStruct(value);
        }

        private string ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);

            reader.Read();
            var value = _stringConverter.Read(ref reader, typeof(string), options);
            return value == null ? value : value[1..^1];
        }

        private void WriteProperty(Utf8JsonWriter writer, JsonEncodedText name, string stringValue, JsonSerializerOptions options)
        {
            writer.WritePropertyName(name);
            // convert the string on write
            if (stringValue != null)
            {
                stringValue = $"*{stringValue}*";
            }
            _stringConverter.Write(writer, stringValue, options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            SecureStruct secureValue,
            JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            WriteProperty(writer, ValueName, secureValue.Value, options);
            writer.WriteEndObject();
        }
    }
}