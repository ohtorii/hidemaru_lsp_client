using Newtonsoft.Json;
/*using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;*/

namespace LSP.Model //OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(TextEditOrInsertReplaceEditConverter))]
    //[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    //[GenerateContainer]
    class TextEditOrInsertReplaceEdit
    {
        private TextEdit/*?*/ _textEdit;
        private InsertReplaceEdit/*?*/ _insertReplaceEdit;

        public TextEditOrInsertReplaceEdit(TextEdit value)
        {
            _textEdit = value;
            _insertReplaceEdit = default;
        }

        public TextEditOrInsertReplaceEdit(InsertReplaceEdit value)
        {
            _textEdit = default;
            _insertReplaceEdit = value;
        }

        public bool IsInsertReplaceEdit => _insertReplaceEdit != null;

        public InsertReplaceEdit/*?*/ InsertReplaceEdit
        {
            get => _insertReplaceEdit;
            set
            {
                _insertReplaceEdit = value;
                _textEdit = null;
            }
        }

        public bool IsTextEdit => _textEdit != null;

        public TextEdit/*?*/ TextEdit
        {
            get => _textEdit;
            set
            {
                _insertReplaceEdit = default;
                _textEdit = value;
            }
        }

        public object/*?*/ RawValue
        {
            get
            {
                if (IsTextEdit) return TextEdit;
                if (IsInsertReplaceEdit) return InsertReplaceEdit;
                return default;
            }
        }

        public static TextEditOrInsertReplaceEdit From(TextEdit value) { return value is null ? null : new TextEditOrInsertReplaceEdit(value); }
        public static implicit operator TextEditOrInsertReplaceEdit(TextEdit value) { return value is null ? null : new TextEditOrInsertReplaceEdit(value); }

        public static TextEditOrInsertReplaceEdit From(InsertReplaceEdit value) { return value is null ? null : new TextEditOrInsertReplaceEdit(value); }
        public static implicit operator TextEditOrInsertReplaceEdit(InsertReplaceEdit value) { return value is null ? null : new TextEditOrInsertReplaceEdit(value); }

        //private string DebuggerDisplay => $"{( IsInsertReplaceEdit ? $"insert: {InsertReplaceEdit}" : IsTextEdit ? $"edit: {TextEdit}" : "..." )}";

        /// <inheritdoc />
        //public override string ToString() => DebuggerDisplay;
    }

}
