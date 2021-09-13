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

        static void Register(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath, bool perUser)
        {
            // Register local server
            Trace.WriteLine($"Registering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.WriteLine($"Executable: {exePath}");
            Trace.Unindent();

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
        }
        public static void UnregisterFromLocalMachine(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid,progId, tlbPath,false);
        }
        public static void UnregisterToCurrentUser(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid,progId, tlbPath,true);
        }
        static void Unregister(Guid clsid, ProgIdAttribute progId, string tlbPath, bool perUser)
        {
            Trace.WriteLine($"Unregistering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

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
        }

        private readonly List<int> registrationCookies = new List<int>();

        public void RegisterClass<T>(Guid clsid) where T : new()
        {
            Trace.WriteLine($"[Create]Registering class object:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.WriteLine($"Type: {typeof(T)}");

            int cookie;
            int hr = Ole32.CoRegisterClassObject(ref clsid, new BasicClassFactory<T>(), Ole32.CLSCTX_LOCAL_SERVER, Ole32.REGCLS_MULTIPLEUSE | Ole32.REGCLS_SUSPENDED, out cookie);
            if (hr < 0)
            {
                Trace.WriteLine("Exception @1");
                Marshal.ThrowExceptionForHR(hr);
            }

            registrationCookies.Add(cookie);
            Trace.WriteLine($"Cookie: {cookie}");
            Trace.Unindent();

            hr = Ole32.CoResumeClassObjects();
            if (hr < 0)
            {
                Trace.WriteLine("Exception @2");
                Marshal.ThrowExceptionForHR(hr);
            }
            Trace.WriteLine($"[Finish]Registering class object:");
        }

        public void Run()
        {
            // This sample does not handle lifetime management of the server.
            // For details around ref counting and locking of out-of-proc COM servers, see
            // https://docs.microsoft.com/windows/win32/com/out-of-process-server-implementation-helpers
            Trace.WriteLine($"================================");
            Trace.WriteLine($"Press ENTER to exit.");
            Trace.WriteLine($"================================");
            Console.ReadLine();
        }

        public void Dispose()
        {
            Trace.WriteLine($"Revoking class object registrations:");
            Trace.Indent();
            foreach (int cookie in registrationCookies)
            {
                Trace.WriteLine($"Cookie: {cookie}");
                int hr = Ole32.CoRevokeClassObject(cookie);
                Debug.Assert(hr >= 0, $"CoRevokeClassObject failed ({hr:x}). Cookie: {cookie}");
            }

            Trace.Unindent();
        }
    }
}

