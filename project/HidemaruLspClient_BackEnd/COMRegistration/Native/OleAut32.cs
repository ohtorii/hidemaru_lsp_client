using System;
using System.Runtime.InteropServices;

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace COMRegistration
{
    internal class OleAut32
    {
        // https://docs.microsoft.com/windows/api/oleauto/ne-oleauto-regkind
        public enum REGKIND
        {
            REGKIND_DEFAULT = 0,
            REGKIND_REGISTER = 1,
            REGKIND_NONE = 2
        }

        // https://docs.microsoft.com/windows/api/oleauto/nf-oleauto-loadtypelibex
        [DllImport(nameof(OleAut32), CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int LoadTypeLibEx(
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            REGKIND regKind,
            out ComTypes.ITypeLib typeLib);

        [DllImport(nameof(OleAut32), CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int LoadTypeLib(
            [In, MarshalAs(UnmanagedType.LPWStr)] string fileName,
            out ComTypes.ITypeLib typeLib);

        [DllImport(nameof(OleAut32), CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int RegisterTypeLib(
            ComTypes.ITypeLib ptlib,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszFullPath,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszHelpDir);

        [DllImport(nameof(OleAut32), CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int RegisterTypeLibForUser(
            ComTypes.ITypeLib ptlib,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszFullPath,
            [In, MarshalAs(UnmanagedType.LPWStr)] string pwszHelpDir);

        // https://docs.microsoft.com/windows/api/oleauto/nf-oleauto-unregistertypelib
        [DllImport(nameof(OleAut32))]
        public static extern int UnRegisterTypeLib(
            ref Guid id,
            short majorVersion,
            short minorVersion,
            int lcid,
            ComTypes.SYSKIND sysKind);

        [DllImport(nameof(OleAut32))]
        public static extern int UnRegisterTypeLibForUser(
            ref Guid id,
            short majorVersion,
            short minorVersion,
            int lcid,
            ComTypes.SYSKIND sysKind);
    }
}
