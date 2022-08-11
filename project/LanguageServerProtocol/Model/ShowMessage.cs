namespace LSP.Model
{
    /**
	 * Show message request client capabilities
	 */
    class ShowMessageRequestClientCapabilities
    {
        /**
		 * Capabilities specific to the `MessageActionItem` type.
		 */
        public class _messageActionItem
        {
            /**
			 * Whether the client supports additional attributes which
			 * are preserved and sent back to the server in the
			 * request's response.
			 */
            public bool additionalPropertiesSupport;
        };
        public _messageActionItem messageActionItem;
    }
    class ShowMessageRequestParams
    {
        /**
		 * The message type. See {@link MessageType}
		 */
        public MessageType type;

        /**
		 * The actual message
		 */
        public string message;

        /**
		 * The message action items to present.
		 */
        public MessageActionItem[] actions;
    }
    class MessageActionItem
    {
        /**
		 * A short title like 'Retry', 'Open Log' etc.
		 */
        public string title;
    }

}
