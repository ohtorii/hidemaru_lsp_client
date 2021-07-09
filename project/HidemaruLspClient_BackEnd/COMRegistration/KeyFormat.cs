namespace COMRegistration
{
    internal static class KeyFormat
    {
        public const string CLSID = @"SOFTWARE\Classes\CLSID\{0:B}";
        public const string AppID = @"SOFTWARE\Classes\AppID\{0:B}";
        public const string ProgId= @"SOFTWARE\Classes\{0}";
        public const string ProgIdCLSID = ProgId+@"\CLSID";

        public static readonly string LocalServer32 = $"{CLSID}\\LocalServer32";
    }
}
