using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
//using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace LSP.Model /*OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters*/
{
    class StringOrMarkupContentConverter : JsonConverter<StringOrMarkupContent>
    {
        public override void WriteJson(JsonWriter writer, StringOrMarkupContent value, JsonSerializer serializer)
        {
            if (value.HasString)
            {
                writer.WriteValue(value.String);
            }
            else
            {
                serializer.Serialize(writer, value.MarkupContent);
            }
        }

        public override StringOrMarkupContent ReadJson(JsonReader reader, Type objectType, StringOrMarkupContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var result = JObject.Load(reader);
                return new StringOrMarkupContent(
                    new MarkupContent
                    {
                        kind = Enum.TryParse<MarkupKind>(result["kind"]?.Value<string>(), true, out var kind) ? kind : MarkupKind.plaintext,
                        value = result["value"].Value<string>()
                    }
                );
            }

            if (reader.TokenType == JsonToken.String)
            {
                return new StringOrMarkupContent(reader.Value as string);
            }

            return "";
        }

        public override bool CanRead => true;
    }
}
