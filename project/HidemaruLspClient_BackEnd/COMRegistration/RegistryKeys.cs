namespace HidemaruLspClient.ComRegistration
{
    internal static class RegistryKeys
    {
        public const string Classes = @"SOFTWARE\Classes";
        public const string CLSID = Classes + @"\CLSID";
        public const string formatCLSID = CLSID + @"\{0:B}";
        public const string formatAppID = @"SOFTWARE\Classes\AppID\{0:B}";
        public const string formatProgId = @"SOFTWARE\Classes\{0}";
        public const string formatProgIdCLSID = formatProgId + @"\CLSID";

        public static readonly string formatLocalServer32 = $"{formatCLSID}\\LocalServer32";
    }
}
