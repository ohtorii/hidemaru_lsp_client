﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentColorClientCapabilities
	{
		/**
		 * Whether document color supports dynamic registration.
		 */
		public bool dynamicRegistration;
	}
#if false
	export interface DocumentColorOptions extends WorkDoneProgressOptions {
	}
	export interface DocumentColorRegistrationOptions extends
		TextDocumentRegistrationOptions, StaticRegistrationOptions,
		DocumentColorOptions {
	}
	interface DocumentColorParams extends WorkDoneProgressParams,
		PartialResultParams {
		/**
		 * The text document.
		 */
		textDocument: TextDocumentIdentifier;
	}
	interface ColorInformation {
		/**
		 * The range in the document where this color appears.
		 */
		range: Range;

		/**
		 * The actual color value for this color range.
		 */
		color: Color;
	}

	/**
	 * Represents a color in RGBA space.
	 */
	interface Color {

		/**
		 * The red component of this color in the range [0-1].
		 */
		readonly red: decimal;

		/**
		 * The green component of this color in the range [0-1].
		 */
		readonly green: decimal;

		/**
		 * The blue component of this color in the range [0-1].
		 */
		readonly blue: decimal;

		/**
		 * The alpha component of this color in the range [0-1].
		 */
		readonly alpha: decimal;
	}
#endif
}
