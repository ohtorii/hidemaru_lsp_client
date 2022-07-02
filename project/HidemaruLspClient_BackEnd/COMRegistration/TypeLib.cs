using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HidemaruLspClient.Native;

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace HidemaruLspClient.ComRegistration
{
    internal static class TypeLib
    {
        /// <summary>
        /// Registering type library:
        /// </summary>
        /// <param name="tlbPath"></param>
        /// <param name="perUser"></param>
        public static void Register(string tlbPath, bool perUser)
        {
            Trace.WriteLine("[Enter]TypeLib.Register");
            Trace.Indent();
            try
            {
                Trace.WriteLine(tlbPath);
            }
            finally
            {
                Trace.Unindent();
            }

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
            Trace.WriteLine("[Leave]TypeLib.Register");
        }
        /// <summary>
        /// Unregistering type library
        /// </summary>
        /// <param name="tlbPath"></param>
        /// <param name="perUser"></param>
        public static void Unregister(string tlbPath, bool perUser)
        {
            Trace.WriteLine("[Enter]TypeLib.Unregister");
            Trace.Indent();
            try
            {
                Trace.WriteLine(tlbPath);
            }
            finally
            {
                Trace.Unindent();
            }

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
            Trace.WriteLine("[Leave]TypeLib.Unregister");
        }
    }
}
