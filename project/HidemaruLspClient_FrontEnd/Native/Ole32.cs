using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd.Native
{
    class Ole32
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
