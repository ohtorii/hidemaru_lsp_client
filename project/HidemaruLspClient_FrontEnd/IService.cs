using HidemaruLspClient_FrontEnd.BackEndContractImpl;
using System.Runtime.InteropServices;

namespace HidemaruLspClient_FrontEnd
{
	[ComVisible(true)]
	[Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9591")]
	interface IService
    {
        int Add(int x, int y);
        void Finalizer(int reason = 0);
		HidemaruLspClient_BackEndContract.IServerCapabilities ServerCapabilities();
        bool SyncDocument();
        string Completion(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Declaration(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Definition(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl TypeDefinition(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl Implementation(long hidemaruLine, long hidemaruColumn);
        LocationContainerImpl References(long hidemaruLine, long hidemaruColumn);
    }
}
