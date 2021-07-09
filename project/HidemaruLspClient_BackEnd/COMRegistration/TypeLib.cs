using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace COMRegistration
{
    internal static class TypeLib
    {
        public static void Register(string tlbPath, bool perUser)
        {
            Trace.WriteLine($"Registering type library:");
            Trace.Indent();
            Trace.WriteLine(tlbPath);
            Trace.Unindent();

            if (perUser)
            {
                ComTypes.ITypeLib typeLib;
                int hr = OleAut32.LoadTypeLibEx(tlbPath, OleAut32.REGKIND.REGKIND_NONE, out typeLib);
                if (hr < 0)
                {
                    Trace.WriteLine($"LoadTypeLib(for user) type library failed: 0x{hr:x}");
                    Marshal.ThrowExceptionForHR(hr);
                }
                hr = OleAut32.RegisterTypeLibForUser(typeLib, tlbPath, null);
                if (hr < 0)
                {
                    Trace.WriteLine($"RegisterType(for user) type library failed: 0x{hr:x}");
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
            else
            {
                int hr = OleAut32.LoadTypeLibEx(tlbPath, OleAut32.REGKIND.REGKIND_REGISTER, out ComTypes.ITypeLib _);
                if (hr < 0)
                {
                    Trace.WriteLine($"Registering type library failed: 0x{hr:x}");
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }
        
        public static void Unregister(string tlbPath, bool perUser)
        {
            Trace.WriteLine($"Unregistering type library:");
            Trace.Indent();
            Trace.WriteLine(tlbPath);
            Trace.Unindent();

            ComTypes.ITypeLib typeLib;
            int hr = OleAut32.LoadTypeLibEx(tlbPath, OleAut32.REGKIND.REGKIND_NONE, out typeLib);
            if (hr < 0)
            {
                Trace.WriteLine($"Unregistering type library failed: 0x{hr:x}");
                return;
            }

            IntPtr attrPtr = IntPtr.Zero;
            try
            {
                typeLib.GetLibAttr(out attrPtr);
                if (attrPtr != IntPtr.Zero)
                {
                    ComTypes.TYPELIBATTR attr = Marshal.PtrToStructure<ComTypes.TYPELIBATTR>(attrPtr);
                    if (perUser)
                    {
                        hr = OleAut32.UnRegisterTypeLibForUser(ref attr.guid, attr.wMajorVerNum, attr.wMinorVerNum, attr.lcid, attr.syskind);
                    }
                    else
                    {
                        hr = OleAut32.UnRegisterTypeLib(ref attr.guid, attr.wMajorVerNum, attr.wMinorVerNum, attr.lcid, attr.syskind);
                    }
                    if (hr < 0)
                    {
                        Trace.WriteLine($"Unregistering type library failed: 0x{hr:x}");
                    }
                }
            }
            finally
            {
                if (attrPtr != IntPtr.Zero)
                {
                    typeLib.ReleaseTLibAttr(attrPtr);
                }
            }
        }

        private class OleAut32
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
}
