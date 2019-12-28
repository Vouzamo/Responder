using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Vouzamo.Responder.App.Models.Rules;

namespace Vouzamo.Responder.App.Converters
{
    public class RuleInputConverter : JsonConverter<RuleInput>
    {
        public RuleInputConverter()
        {
            
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RuleInput);
        }

        public override RuleInput Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);

            var root = document.RootElement;

            if (document.RootElement.TryGetProperty("Type", out JsonElement typeProperty))
            {
                var typeValue = typeProperty.GetString();

                switch(typeValue)
                {
                    case "string": return JsonSerializer.Deserialize<StringRuleInput>(root.GetRawText(), options);
                    case "select": return JsonSerializer.Deserialize<SelectRuleInput>(root.GetRawText(), options);
                };
            }

            return default(RuleInput);
        }

        public override void Write(Utf8JsonWriter writer, RuleInput value, JsonSerializerOptions options)
        {
            var serialized = JsonSerializer.Serialize(value, value.GetType()).Trim('"');

            writer.WriteStartObject();

            using var doc = JsonDocument.Parse(serialized);

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();
        }
    }

    public class ObjectToPrimitiveConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    // If an offset was provided, use DateTimeOffset.
                    if (datetime.Kind == DateTimeKind.Local)
                    {
                        if (reader.TryGetDateTimeOffset(out DateTimeOffset datetimeOffset))
                        {
                            return datetimeOffset;
                        }
                    }

                    return datetime;
                }

                return reader.GetString();
            }

            // Use JsonElement as fallback.
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new InvalidOperationException("Should not get here.");
        }
    }
}
