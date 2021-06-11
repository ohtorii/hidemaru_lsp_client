using System;
using System.Collections.Generic;
using System.Text;

namespace LSP.Model
{
	class DocumentLinkClientCapabilities
	{
		/**
		 * Whether document link supports dynamic registration.
		 */
		public bool dynamicRegistration;

		/**
		 * Whether the client supports the `tooltip` property on `DocumentLink`.
		 *
		 * @since 3.15.0
		 */
		public bool tooltipSupport;
	}


	interface IDocumentLinkOptions :IWorkDoneProgressOptions
	{
		/**
		 * Document links have a resolve provider as well.
		 */
		bool resolveProvider { get; set; }
	}
	interface IDocumentLinkRegistrationOptions :ITextDocumentRegistrationOptions, IDocumentLinkOptions 
	{
	}

	interface IDocumentLinkParams : IWorkDoneProgressParams,IPartialResultParams {
		/**
		 * The document to provide document links for.
		 */
		ITextDocumentIdentifier textDocument { get; set; }
	}



#if false
/**
 * A document link is a range in a text document that links to an internal or
 * external resource, like another text document or a web site.
 */
interface DocumentLink
{
	/**
	 * The range this link applies to.
	 */
	range: Range;

	/**
	 * The uri this link points to. If missing a resolve request is sent later.
	 */
	target?: DocumentUri;

	/**
	 * The tooltip text when you hover over this link.
	 *
	 * If a tooltip is provided, is will be displayed in a string that includes
	 * instructions on how to trigger the link, such as `{0} (ctrl + click)`.
	 * The specific instructions vary depending on OS, user settings, and
	 * localization.
	 *
	 * @since 3.15.0
	 */
	tooltip?: string;

	/**
	 * A data entry field that is preserved on a document link between a
	 * DocumentLinkRequest and a DocumentLinkResolveRequest.
	 */
	data?: any;
}
#endif


	class DocumentLinkOptions : IDocumentLinkOptions
	{
		public bool resolveProvider { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
