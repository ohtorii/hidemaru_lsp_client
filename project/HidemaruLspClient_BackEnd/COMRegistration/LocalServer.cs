using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace COMRegistration
{
    sealed class LocalServer : IDisposable
    {
        public static void RegisterToLocalMachine(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath)
        {
            Register(clsid, progId, exePath, tlbPath, false);
        }
        public static void RegisterToCurrentUser(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath)
        {
            Register(clsid, progId, exePath, tlbPath, true);
        }
        /// <summary>
        /// Registering server
        /// </summary>
        /// <param name="clsid"></param>
        /// <param name="progId"></param>
        /// <param name="exePath"></param>
        /// <param name="tlbPath"></param>
        /// <param name="perUser"></param>
        static void Register(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath, bool perUser)
        {
            // Register local server
            Trace.WriteLine("[Enter]LocalServer.Register");
            Trace.Indent();
            try
            {
                Trace.WriteLine($"CLSID     : {clsid:B}");
                Trace.WriteLine($"progId    : {progId?.Value}");
                Trace.WriteLine($"Executable: {exePath}");
                Trace.WriteLine($"tlbPath   : {tlbPath}");
                Trace.WriteLine($"perUser   : {perUser}");
            }
            finally
            {
                Trace.Unindent();
            }

            RegistryKey dst;
            if (perUser)
            {
                dst = Registry.CurrentUser;
            }
            else
            {
                dst = Registry.LocalMachine;
            }
            string serverKey = string.Format(KeyFormat.LocalServer32, clsid);
            using (var regKey = dst.CreateSubKey(serverKey))
            {
                regKey.SetValue(null, exePath);
            }

            if (progId != null)
            {//Register ProgId
                var progIdKey = string.Format(KeyFormat.ProgIdCLSID, progId.Value);
                using (var sub = dst.CreateSubKey(progIdKey))
                {
                    sub.SetValue(null, string.Format("{{{0}}}", clsid));
                }
            }
            if ((tlbPath != null) && (tlbPath != ""))
            {
                TypeLib.Register(tlbPath, perUser);
            }
            Trace.WriteLine("[Leave]LocalServer.Register");
        }
        public static void UnregisterFromLocalMachine(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid,progId, tlbPath,false);
        }
        public static void UnregisterToCurrentUser(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid,progId, tlbPath,true);
        }
        /// <summary>
        /// Unregistering server
        /// </summary>
        /// <param name="clsid"></param>
        /// <param name="progId"></param>
        /// <param name="tlbPath"></param>
        /// <param name="perUser"></param>
        static void Unregister(Guid clsid, ProgIdAttribute progId, string tlbPath, bool perUser)
        {
            Trace.WriteLine("[Enter]LocalServer.Unregister");
            Trace.Indent();
            try
            {
                Trace.WriteLine($"CLSID: {clsid:B}");
                Trace.WriteLine($"progId    : {progId?.Value}");
                Trace.WriteLine($"tlbPath   : {tlbPath}");
                Trace.WriteLine($"perUser   : {perUser}");
            }
            finally
            {
                Trace.Unindent();
            }

            RegistryKey dst;
            if (perUser)
            {
                dst = Registry.CurrentUser;
            }
            else
            {
                dst = Registry.LocalMachine;
            }
            // Unregister local server
            {
                string serverKey = string.Format(KeyFormat.CLSID, clsid);
                dst.DeleteSubKeyTree(serverKey, throwOnMissingSubKey: false);
            }

            if (progId!=null)
            {
                //Unregister ProgId
                var progIdKey = string.Format(KeyFormat.ProgId, progId.Value);
                dst.DeleteSubKeyTree(progIdKey, throwOnMissingSubKey: false);
            }
            if ((tlbPath != null) && (tlbPath != ""))
            {
                TypeLib.Unregister(tlbPath, perUser);
            }
            Trace.WriteLine("[Leave]LocalServer.Unregister");
        }

        private readonly List<int> registrationCookies = new List<int>();
        /// <summary>
        /// Registering class object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clsid"></param>
        public void RegisterClass<T>(Guid clsid) where T : new()
        {
            Trace.WriteLine("[Enter]LocalServer.RegisterClass");
            Trace.Indent();
            try
            {
                Trace.WriteLine($"CLSID: {clsid:B}");
                Trace.WriteLine($"Type: {typeof(T)}");

                int cookie;
                int hrRegister = Ole32.CoRegisterClassObject(ref clsid, new BasicClassFactory<T>(), Ole32.CLSCTX_LOCAL_SERVER, Ole32.REGCLS_MULTIPLEUSE | Ole32.REGCLS_SUSPENDED, out cookie);
                if (hrRegister < 0)
                {
                    Trace.WriteLine(string.Format($"Marshal.ThrowExceptionForHR(hrRegister={hrRegister})"));
                    Marshal.ThrowExceptionForHR(hrRegister);
                }
                Trace.WriteLine($"Cookie: {cookie}");
                registrationCookies.Add(cookie);
            }
            finally
            {
                Trace.Unindent();
            }

            int hrResume = Ole32.CoResumeClassObjects();
            if (hrResume < 0)
            {
                Trace.WriteLine($"Marshal.ThrowExceptionForHR(hrResume={hrResume})");
                Marshal.ThrowExceptionForHR(hrResume);
            }
            Trace.WriteLine("[Leave]LocalServer.RegisterClass");
        }

        /// <summary>
        /// Revoking class object registrations
        /// </summary>
        public void Dispose()
        {
            Trace.WriteLine("[Enter]LocalServer.Dispose");
            Trace.Indent();
            try
            {
                foreach (int cookie in registrationCookies)
                {
                    Trace.WriteLine($"Cookie: {cookie}");
                    int hr = Ole32.CoRevokeClassObject(cookie);
                    if (hr < 0) {
                        Trace.TraceError($"CoRevokeClassObject failed ({hr:x}). Cookie: {cookie}");
                    }
                }
            }
            finally
            {
                Trace.Unindent();
            }
            Trace.WriteLine("[Leave]LocalServer.Dispose");
        }
    }
}

