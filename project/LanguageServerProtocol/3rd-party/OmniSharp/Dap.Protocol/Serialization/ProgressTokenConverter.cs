using Newtonsoft.Json;
using System;
//using OmniSharp.Extensions.DebugAdapter.Protocol.Models;

namespace LSP.Model //mniSharp.Extensions.DebugAdapter.Protocol.Serialization
{
    internal class ProgressTokenConverter : JsonConverter<ProgressToken>
    {
        public override void WriteJson(JsonWriter writer, ProgressToken value, JsonSerializer serializer)
        {
            if (value == null) writer.WriteNull();
            else if (value.IsLong) serializer.Serialize(writer, value.Long);
            else if (value.IsString) serializer.Serialize(writer, value.String);
            else writer.WriteNull();
        }

        public override ProgressToken ReadJson(JsonReader reader, Type objectType, ProgressToken existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
#if true
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    return new ProgressToken((long)reader.Value);
                case JsonToken.String when reader.Value is string str && !string.IsNullOrWhiteSpace(str):
                    return new ProgressToken(str);
                default:
                    return null;
            }
#else
            //C# 8.0
            return reader.TokenType switch {
                JsonToken.Integer                                                                   => new ProgressToken((long) reader.Value),
                JsonToken.String when reader.Value is string str && !string.IsNullOrWhiteSpace(str) => new ProgressToken(str),
                _                                                                                   => null
            };
#endif
        }

        public override bool CanRead => true;
    }
}
