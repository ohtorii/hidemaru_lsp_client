using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HidemaruLspClient_FrontEnd.Native
{
    class IniFileNative
    {
        string Path_;
        public string Filename => Path_;

        public IniFileNative(string IniPath)
        {
            Path_ = new FileInfo(IniPath).FullName;
        }

        public string Read(string Key, string Section)
        {
            const int capacity = 256;
            var RetVal = new StringBuilder(capacity);
            Kernel32.GetPrivateProfileString(Section, Key, "", RetVal, RetVal.Capacity, Path_);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section)
        {
            Kernel32.WritePrivateProfileString(Section, Key, Value, Path_);
        }

        public void DeleteKey(string Key, string Section)
        {
            Write(Key, null, Section);
        }

        public void DeleteSection(string Section)
        {
            Write(null, null, Section);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return 0 < Read(Key, Section).Length;
        }
    }
}
