namespace LSP.Model
{
    class WorkspaceSymbolClientCapabilities
    {
        public bool dynamicRegistration;
        public class _symbolKind
        {
            public SymbolKind[] valueSet;
        };
        public _symbolKind symbolKind;

        /**
		 * The client supports tags on `SymbolInformation`.
		 * Clients supporting tags have to handle unknown tags gracefully.
		 *
		 * @since 3.16.0
		 */
        public class _tagSupport
        {
            /**
			 * The tags supported by the client.
			 */
            public SymbolTag[] valueSet;
        };
        public _tagSupport tagSupport;
    }

    interface IWorkspaceSymbolOptions : IWorkDoneProgressOptions
    {
    }
    interface IWorkspaceSymbolRegistrationOptions : IWorkspaceSymbolOptions
    {
    }
    class WorkspaceSymbolOptions : IWorkspaceSymbolOptions
    {
        public bool workDoneProgress { get; set; }
    }
    class WorkspaceSymbolRegistrationOptions : IWorkspaceSymbolRegistrationOptions
    {
        public bool workDoneProgress { get; set; }
    }
}
