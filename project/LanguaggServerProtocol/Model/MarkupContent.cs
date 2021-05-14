using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace LSP.Model
{
	//Todo: 名前をPlainText,Markdownにする。
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MarkupKind
	{
		/// <summary>
		/// Plain text is supported as a content format
		/// </summary>
		plaintext=0, 
		/// <summary>
		/// Markdown is supported as a content format
		/// </summary>
		markdown=1,
	}
	

	/**
	 * A `MarkupContent` literal represents a string value which content is
	 * interpreted base on its kind flag. Currently the protocol supports
	 * `plaintext` and `markdown` as markup kinds.
	 *
	 * If the kind is `markdown` then the value can contain fenced code blocks like
	 * in GitHub issues.
	 *
	 * Here is an example how such a string can be constructed using
	 * JavaScript / TypeScript:
	 * ```typescript
	 * let markdown: MarkdownContent = {
	 * 	kind: MarkupKind.Markdown,
	 * 	value: [
	 * 		'# Header',
	 * 		'Some text',
	 * 		'```typescript',
	 * 		'someCode();',
	 * 		'```'
	 * 	].join('\n')
	 * };
	 * ```
	 *
	 * *Please Note* that clients might sanitize the return markdown. A client could
	 * decide to remove HTML from the markdown to avoid script execution.
	 */
	class MarkupContent
	{
		/**
		* The type of the Markup
		*/
		public MarkupKind kind;

		/**
		 * The content itself
		 */
		public string value;
	}
	/**
 * Client capabilities specific to the used markdown parser.
 *
 * @since 3.16.0
 */
	class MarkdownClientCapabilities
	{
		/**
		 * The name of the parser.
		 */
		public string parser;

		/**
		 * The version of the parser.
		 */
		public string version;
	}
}
