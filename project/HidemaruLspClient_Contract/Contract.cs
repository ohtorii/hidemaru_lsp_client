using System;

namespace LspContract
{
    internal sealed class Constants
    {
        public const string ServerClass = "EF516543-D040-46CC-88B3-FD64C09DB652";
        public static readonly Guid ServerClassGuid = Guid.Parse(ServerClass);

        public const string ServerInterface = "9159F305-06A3-4231-B023-BFF30CE0D9E8";
        public const string ProgId = "HidemaruLSPClient_BackEnd.HidemaruLspBackEndServer";
        //public const string TypeLibraryName = "Server.Contract.tlb";
    }
}
