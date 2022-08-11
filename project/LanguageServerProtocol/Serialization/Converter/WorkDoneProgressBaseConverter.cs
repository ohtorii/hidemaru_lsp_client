using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace LSP.Serialization.Converter
{
    class WorkDoneProgressBaseConverter : JsonConverter<WorkDoneProgressBase>
    {
        public override void WriteJson(JsonWriter writer, WorkDoneProgressBase value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
        public override WorkDoneProgressBase ReadJson(JsonReader reader, Type objectType, WorkDoneProgressBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var result = JObject.Load(reader);
            var kind = result["kind"].ToString();
            var cancellable = result["cancellable"];
            var title = result["title"];
            var message = result["message"];
            var percentage = result["percentage"];

            switch (kind)
            {
                case "begin":
                    return new WorkDoneProgressBegin
                    {
                        kind = kind,
                        cancellable = cancellable != null && cancellable.Value<bool>(),
                        title = title == null ? "" : title.ToString(),
                        message = message == null ? "" : message.ToString(),
                        percentage = percentage == null ? 0 : percentage.Value<uint>()
                    };
                case "report":
                    return new WorkDoneProgressReport
                    {
                        kind = kind,
                        cancellable = cancellable != null && cancellable.Value<bool>(),
                        message = message == null ? "" : message.ToString(),
                        percentage = percentage == null ? 0 : percentage.Value<uint>()
                    };
                case "end":
                    return new WorkDoneProgressEnd
                    {
                        kind = kind,
                        message = message == null ? "" : message.ToString()
                    };
                default:
                    throw new NotImplementedException();
            }
            //return null;
        }
    }
}
