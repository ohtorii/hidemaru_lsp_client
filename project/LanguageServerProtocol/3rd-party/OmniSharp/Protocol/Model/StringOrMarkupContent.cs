using Newtonsoft.Json;
//using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace LSP.Model /*OmniSharp.Extensions.LanguageServer.Protocol.Models*/
{
    [JsonConverter(typeof(StringOrMarkupContentConverter))]
    //[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    class StringOrMarkupContent
    {
        public StringOrMarkupContent(string value) => String = value;

        public StringOrMarkupContent(MarkupContent markupContent) => MarkupContent = markupContent;

        public string/*?*/ String { get; }
        public bool HasString => MarkupContent == null;
        public MarkupContent/*?*/ MarkupContent { get; }
        public bool HasMarkupContent => String == null;

        public static implicit operator StringOrMarkupContent(string value)
        {
            return value is null ? null : new StringOrMarkupContent(value);
        }

        public static implicit operator StringOrMarkupContent(MarkupContent markupContent)
        {
            return markupContent is null ? null : new StringOrMarkupContent(markupContent);
        }

        /*private string DebuggerDisplay => $"{( HasString ? String : HasMarkupContent ? MarkupContent!.ToString() : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
        */
    }
}
