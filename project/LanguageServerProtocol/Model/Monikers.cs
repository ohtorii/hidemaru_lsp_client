namespace LSP.Model
{

	interface IMonikerClientCapabilities
	{
		/**
		 * Whether implementation supports dynamic registration. If this is set to
		 * `true` the client supports the new `(TextDocumentRegistrationOptions &
		 * StaticRegistrationOptions)` return value for the corresponding server
		 * capability as well.
		 */
		bool dynamicRegistration { get; set; }
	}
	interface IMonikerOptions : IWorkDoneProgressOptions
	{
	}
	interface IMonikerRegistrationOptions : ITextDocumentRegistrationOptions, IMonikerOptions
	{
	}

#if false
export interface MonikerParams extends TextDocumentPositionParams,
	WorkDoneProgressParams, PartialResultParams {
}

/**
 * Moniker uniqueness level to define scope of the moniker.
 */
export enum UniquenessLevel {
	/**
	 * The moniker is only unique inside a document
	 */
	document = 'document',

	/**
	 * The moniker is unique inside a project for which a dump got created
	 */
	project = 'project',

	/**
	 * The moniker is unique inside the group to which a project belongs
	 */
	group = 'group',

	/**
	 * The moniker is unique inside the moniker scheme.
	 */
	scheme = 'scheme',

	/**
	 * The moniker is globally unique
	 */
	global = 'global'
}

/**
 * The moniker kind.
 */
export enum MonikerKind {
	/**
	 * The moniker represent a symbol that is imported into a project
	 */
	import = 'import',

	/**
	 * The moniker represents a symbol that is exported from a project
	 */
	export = 'export',

	/**
	 * The moniker represents a symbol that is local to a project (e.g. a local
	 * variable of a function, a class not visible outside the project, ...)
	 */
	local = 'local'
}

/**
 * Moniker definition to match LSIF 0.5 moniker definition.
 */
export interface Moniker {
	/**
	 * The scheme of the moniker. For example tsc or .Net
	 */
	scheme: string;

	/**
	 * The identifier of the moniker. The value is opaque in LSIF however
	 * schema owners are allowed to define the structure if they want.
	 */
	identifier: string;

	/**
	 * The scope in which the moniker is unique
	 */
	unique: UniquenessLevel;

	/**
	 * The moniker kind if known.
	 */
	kind?: MonikerKind;
}

#endif


	class MonikerRegistrationOptions : IMonikerRegistrationOptions
	{
		public DocumentFilter[] documentSelector { get; set; }
		public bool workDoneProgress { get; set; }
	}
}
