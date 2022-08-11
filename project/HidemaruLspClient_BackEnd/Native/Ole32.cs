using System;
using System.Runtime.InteropServices;

namespace HidemaruLspClient.Native
{
    class Ole32
    {
        // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
        public const int CLSCTX_LOCAL_SERVER = 0x4;

        // https://docs.microsoft.com/windows/win32/api/combaseapi/ne-combaseapi-regcls
        public const int REGCLS_MULTIPLEUSE = 1;
        public const int REGCLS_SUSPENDED = 4;

        // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coregisterclassobject
        [DllImport(nameof(Ole32))]
        public static extern int CoRegisterClassObject(ref Guid guid, [MarshalAs(UnmanagedType.IUnknown)] object obj, int context, int flags, out int register);

        // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-coresumeclassobjects
        [DllImport(nameof(Ole32))]
        public static extern int CoResumeClassObjects();

        // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-corevokeclassobject
        [DllImport(nameof(Ole32))]
        public static extern int CoRevokeClassObject(int register);

        [DllImport(nameof(Ole32))]
        public static extern int CoAddRefServerProcess();

        [DllImport(nameof(Ole32))]
        public static extern int CoReleaseServerProcess();
    }
}
