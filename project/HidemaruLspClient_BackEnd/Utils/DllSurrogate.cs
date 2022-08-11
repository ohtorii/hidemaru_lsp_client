using HidemaruLspClient.ComRegistration;
using System;
using System.Diagnostics;


namespace HidemaruLspClient.Utils
{
    public static class DllSurrogate
    {
        public static void Register(Guid clsid, string tlbPath)
        {
            Trace.WriteLine($"Registering server with system-supplied DLL surrogate:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

            string serverKey = string.Format(RegistryKeys.formatCLSID, clsid);

            // Register App ID - use the CLSID as the App ID
            using (var regKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(serverKey))
            {
                regKey.SetValue("AppID", clsid.ToString("B"));
            }

            // Register DLL surrogate - empty string for system-supplied surrogate
            string appIdKey = string.Format(RegistryKeys.formatAppID, clsid);
            using (var appIdRegKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(appIdKey))
            {
                appIdRegKey.SetValue("DllSurrogate", string.Empty);
            }

        }

        public static void Unregister(Guid clsid, string tlbPath)
        {
            Trace.WriteLine($"Unregistering server:");
            Trace.Indent();
            Trace.WriteLine($"CLSID: {clsid:B}");
            Trace.Unindent();

            // Remove the App ID value
            string serverKey = string.Format(RegistryKeys.formatCLSID, clsid);
            using (var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(serverKey, writable: true))
            {
                if (regKey != null)
                {
                    regKey.DeleteValue("AppID");
                }
            }

            // Remove the App ID key
            string appIdKey = string.Format(RegistryKeys.formatAppID, clsid);
            Microsoft.Win32.Registry.LocalMachine.DeleteSubKey(appIdKey, throwOnMissingSubKey: false);


        }
    }
}
