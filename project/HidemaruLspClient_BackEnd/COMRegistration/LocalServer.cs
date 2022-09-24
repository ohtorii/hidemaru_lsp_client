using HidemaruLspClient.Native;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace HidemaruLspClient.ComRegistration
{
    sealed class LocalServer : IDisposable
    {
        public static bool RegisterToLocalMachine(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath)
        {
            return Register(clsid, progId, exePath, tlbPath, false);
        }
        public static bool RegisterToCurrentUser(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath)
        {
            return Register(clsid, progId, exePath, tlbPath, true);
        }
        /// <summary>
        /// Registering server
        /// </summary>
        /// <param name="clsid"></param>
        /// <param name="progId"></param>
        /// <param name="exePath"></param>
        /// <param name="tlbPath"></param>
        /// <param name="perUser"></param>
        static bool Register(Guid clsid, ProgIdAttribute progId, string exePath, string tlbPath, bool perUser)
        {
            // Register local server
            Trace.WriteLine("[Enter]LocalServer.Register");
            Trace.Indent();
            var clsidString = "{"+clsid.ToString().ToUpper()+"}";
            try
            {
                Trace.WriteLine($"CLSID     : {clsidString:B}");
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
                dst = Microsoft.Win32.Registry.CurrentUser;
            }
            else
            {
                dst = Microsoft.Win32.Registry.LocalMachine;
            }
            Trace.WriteLine(string.Format("Target registory={0}", dst.Name));
            var absPath= RegistryKeys.CLSID + @"\" + clsidString+ @"\LocalServer32";
            Trace.WriteLine($"absPath={absPath}");
            using(var key = dst.CreateSubKey(absPath)){
                if (key ==null)
                {
                    Trace.WriteLine($"{absPath} is not found.");
                }
                else
                {
                    Trace.WriteLine($"{absPath} is found.");
                    Trace.WriteLine(string.Format("[Write]\"{0}\" ← \"{1}\"", absPath, exePath));
                    key.SetValue(null, exePath);
                }
            }
            /*
            //"SOFTWARE\Classes\CLSID"
            CreateSubkeyIfNotExists(dst, RegistryKeys.Classes, "CLSID");
            //"SOFTWARE\Classes\CLSID\{...}"
            CreateSubkeyIfNotExists(dst, RegistryKeys.CLSID,clsidString);
            //"SOFTWARE\Classes\CLSID\{...}\LocalServer32"
            CreateSubkeyIfNotExists(dst, RegistryKeys.CLSID+@"\"+clsidString, "LocalServer32");
            */
            /*
            {
                string serverKey = string.Format(RegistryKeys.formatLocalServer32, clsidString);
                using (var regKey = dst.CreateSubKey(serverKey))
                {
                    regKey.SetValue(null, exePath);
                    Trace.WriteLine(string.Format("[Write]\"{0}\" ← \"{1}\"", serverKey, exePath));
                }
            }
            */
            if (progId != null)
            {//Register ProgId
                var progIdKeyName = string.Format(RegistryKeys.formatProgIdCLSID, progId.Value);
                using (var keyProgId = dst.CreateSubKey(progIdKeyName))
                {
                    keyProgId.SetValue(null, clsidString);
                    Trace.WriteLine(string.Format("[Write]{0} ← {1}", keyProgId.Name, clsidString));
                }
            }
            if ((tlbPath != null) && (tlbPath != ""))
            {
                TypeLib.Register(tlbPath, perUser);
            }
            Trace.WriteLine("[Leave]LocalServer.Register");
            return true;
        }
        static bool CreateSubkeyIfNotExists(RegistryKey reg, string parentKeyName, string subKeyName)
        {
            using (var parentReg = reg.OpenSubKey(parentKeyName, true))
            {
                string subKeyAbsPath = parentReg.Name + @"\" + subKeyName;
                if (parentReg.GetSubKeyNames().Contains(subKeyName))
                {
                    Trace.WriteLine(subKeyAbsPath + " is exists.");
                }
                else
                {
                    Trace.WriteLine(subKeyAbsPath + " is not exists.");
                    using (var keyCLSID = parentReg.CreateSubKey(subKeyName))
                    {
                        if (keyCLSID == null)
                        {
                            Trace.WriteLine(subKeyAbsPath + " was not created.");
                            return false;
                        }
                        else
                        {
                            keyCLSID.Flush();
                            Trace.WriteLine(keyCLSID.Name + " was created.");
                        }
                    }
                }
            }
            return true;
        }
        public static void UnregisterFromLocalMachine(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid, progId, tlbPath, false);
        }
        public static void UnregisterToCurrentUser(Guid clsid, ProgIdAttribute progId, string tlbPath)
        {
            Unregister(clsid, progId, tlbPath, true);
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
                dst = Microsoft.Win32.Registry.CurrentUser;
            }
            else
            {
                dst = Microsoft.Win32.Registry.LocalMachine;
            }
            // Unregister local server
            {
                string serverKey = string.Format(RegistryKeys.formatCLSID, clsid);
                dst.DeleteSubKeyTree(serverKey, throwOnMissingSubKey: false);
            }

            if (progId != null)
            {
                //Unregister ProgId
                var progIdKey = string.Format(RegistryKeys.formatProgId, progId.Value);
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
                    if (hr < 0)
                    {
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

