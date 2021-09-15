using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace HidemaruLspClient_FrontEnd
{
    class IniFile
    {
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        string Path_;
        public string Filename => Path_;

        public IniFile(string IniPath)
        {
            Path_ = new FileInfo(IniPath).FullName;
        }

        public string Read(string Key, string Section)
        {
            const int capacity = 256;
            var RetVal = new StringBuilder(capacity);
            GetPrivateProfileString(Section, Key, "", RetVal, RetVal.Capacity, Path_);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section)
        {
            WritePrivateProfileString(Section, Key, Value, Path_);
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
