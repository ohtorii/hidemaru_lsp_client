namespace HidemaruLspClient_FrontEnd
{
    interface IService
    {
        int Add(int x, int y);
        void Finalizer(int reason = 0);
        bool SyncDocument();
        string Completion(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl References(long hidemaruLine, long hidemaruColumn);
    }
}
