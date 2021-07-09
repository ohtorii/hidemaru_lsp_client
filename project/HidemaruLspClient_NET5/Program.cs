using System;
using System.Runtime.InteropServices;

namespace HidemaruLspClient_NET5
{
    class Program
    {
        static void Main(string[] args)
        {
            object obj;
            int hr = Ole32.CoCreateInstance(LspContract.Constants.ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IHidemaruLspBackEndServer).GUID, out obj);
            if (hr < 0)
            {
                Marshal.ThrowExceptionForHR(hr);
            }

            var server = (IHidemaruLspBackEndServer)obj;
            var ans = server.Add(1, 2);
            Console.WriteLine($"ans = {ans}");
            //var ret = server.Hoge("MyString");
            //var c = server.Completion("filename",1,2);
        }

        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
            [DllImport(nameof(Ole32))]
            public static extern int CoCreateInstance(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
                IntPtr pUnkOuter,
                uint dwClsContext,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
        }
    }
}
