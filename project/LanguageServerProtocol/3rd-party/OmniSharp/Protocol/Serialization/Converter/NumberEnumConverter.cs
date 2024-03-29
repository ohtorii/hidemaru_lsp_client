﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Lsp.Model.Serialization.Converters
{
    internal class NumberEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => new JValue(value).WriteTo(writer);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => Enum.Parse(
            Nullable.GetUnderlyingType(objectType) ?? objectType, reader.Value.ToString()
        );

        public override bool CanRead => true;

        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsEnum;
    }
}
