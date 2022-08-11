namespace LSP.Model
{

    interface IProgressParams
    {
        /**
		 * The progress token provided by the client or server.
		 */
        ProgressToken token { get; set; }

        /**
		 * The progress data.
		 */
        WorkDoneProgressBase value { get; set; }
    }
    class ProgressParams : IProgressParams
    {
        public ProgressToken token { get; set; }
        public WorkDoneProgressBase value { get; set; }
    }
}
