namespace LSP.Model
{
    interface IDidChangeConfigurationClientCapabilities
    {
        bool dynamicRegistration { get; set; }
    }
    class DidChangeConfigurationClientCapabilities : IDidChangeConfigurationClientCapabilities
    {
        public bool dynamicRegistration { get; set; }
    }
    interface IDidChangeConfigurationParams
    {
        object settings { get; set; }
    }
    class DidChangeConfigurationParams : IDidChangeConfigurationParams
    {
        public object settings { get; set; }
    }
}
