using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
#if false
	enum SemanticTokenTypes
	{
		@namespace = 'namespace',
		/**
		 * Represents a generic type. Acts as a fallback for types which
		 * can't be mapped to a specific type like class or enum.
		 */
		type = 'type',
		class = 'class',
		enum = 'enum',
		interface = 'interface',
		struct = 'struct',
		typeParameter = 'typeParameter',
		parameter = 'parameter',
		variable = 'variable',
		property = 'property',
		enumMember = 'enumMember',
		event = 'event',
		function = 'function',
		method = 'method',
		macro = 'macro',
		keyword = 'keyword',
		modifier = 'modifier',
		comment = 'comment',
		string = 'string',
		number = 'number',
		regexp = 'regexp',
		operator = 'operator'
	}

	 enum SemanticTokenModifiers
	{
		declaration = 'declaration',
		definition = 'definition',
		readonly = 'readonly',
		static = 'static',
		deprecated = 'deprecated',
		abstract = 'abstract',
		async = 'async',
		modification = 'modification',
		documentation = 'documentation',
		defaultLibrary = 'defaultLibrary'
	}

	export namespace TokenFormat
	{
			export const Relative: 'relative' = 'relative';
	}

	export type TokenFormat = 'relative';


	interface ISemanticTokensLegend
	{
		string[]  tokenTypes;

		/**
		 * The token modifiers a server uses.
		 */
		string[]  tokenModifiers;
	}

	interface ISemanticTokensClientCapabilities
	{		
		bool dynamicRegistration;
		class _requests {
			/**
			 * The client will send the `textDocument/semanticTokens/range` request
			 * if the server provides a corresponding handler.
			 */
			range?: boolean | {};
		
			full?: boolean | {
				/**
					* The client will send the `textDocument/semanticTokens/full/delta`
					* request if the server provides a corresponding handler.
					*/
				delta?: boolean
			};
		};

		/**
		 * The token types that the client supports.
		 */
		tokenTypes: string[];

		/**
		 * The token modifiers that the client supports.
		 */
		tokenModifiers: string[];

		/**
		 * The formats the clients supports.
		 */
		formats: TokenFormat[];

		/**
		 * Whether the client supports tokens that can overlap each other.
		 */
		overlappingTokenSupport ?: boolean;

		/**
		 * Whether the client supports tokens that can span multiple lines.
		 */
		multilineTokenSupport ?: boolean;
	}

	export interface SemanticTokensOptions extends WorkDoneProgressOptions
	{
		/**
		 * The legend used by the server
		 */
		legend: SemanticTokensLegend;

		/**
		 * Server supports providing semantic tokens for a specific range
		 * of a document.
		 */
		range?: boolean | {
		};

		/**
		 * Server supports providing semantic tokens for a full document.
		 */
		full?: boolean | {
			/**
			 * The server supports deltas for full documents.
			 */
			delta ?: boolean;
	};

	export interface SemanticTokensRegistrationOptions extends
		TextDocumentRegistrationOptions, SemanticTokensOptions,
		StaticRegistrationOptions {
	}


export interface SemanticTokens
{
	/**
	 * An optional result id. If provided and clients support delta updating
	 * the client will include the result id in the next semantic token request.
	 * A server can then instead of computing all semantic tokens again simply
	 * send a delta.
	 */
	resultId?: string;

	/**
	 * The actual tokens.
	 */
	data: uinteger[];
}

export interface SemanticTokensPartialResult
{
	data: uinteger[];
}


	export interface SemanticTokensDeltaParams extends WorkDoneProgressParams,
		PartialResultParams {
		/**
		 * The text document.
		 */
		textDocument: TextDocumentIdentifier;

		/**
		 * The result id of a previous response. The result Id can either point to
		 * a full response or a delta response depending on what was received last.
		 */
		previousResultId: string;
	}


	export interface SemanticTokensDelta
	{
		readonly resultId?: string;
		/**
		 * The semantic token edits to transform a previous result into a new
		 * result.
		 */
		edits: SemanticTokensEdit[];
	}

	export interface SemanticTokensEdit
	{
		/**
		 * The start offset of the edit.
		 */
		start: uinteger;

		/**
		 * The count of elements to remove.
		 */
		deleteCount: uinteger;

		/**
		 * The elements to insert.
		 */
		data?: uinteger[];
	}

	export interface SemanticTokensDeltaPartialResult
	{
		edits: SemanticTokensEdit[];
	}

	interface ISemanticTokensRangeParams : IWorkDoneProgressParams,
		public class PartialResultParams {
		/**
		 * The text document.
		 */
			TextDocumentIdentifier textDocument;

		/**
		 * The range the semantic tokens are requested for.
		 */
		Range range;
	}
#endif
	class SemanticTokensWorkspaceClientCapabilities
	{
		/**
		 * Whether the client implementation supports a refresh request sent from
		 * the server to the client.
		 *
		 * Note that this event is global and will force the client to refresh all
		 * semantic tokens currently shown. It should be used with absolute care
		 * and is useful for situation where a server for example detect a project
		 * wide change that requires such a calculation.
		 */
		public bool refreshSupport;
	}

}
