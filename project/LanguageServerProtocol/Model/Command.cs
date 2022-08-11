namespace LSP.Model
{
    interface ICommand
    {
        /**
		 * Title of the command, like `save`.
		 */
        string title { get; set; }
        /**
		 * The identifier of the actual command handler.
		 */
        string command { get; set; }
        /**
		 * Arguments that the command handler should be
		 * invoked with.
		 */
        object arguments { get; set; }
    }
    class Command : ICommand
    {
        public string title { get; set; }
        public string command { get; set; }
        public object arguments { get; set; }
    }
}
