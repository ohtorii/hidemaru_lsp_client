using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    class TempFile
    {
        static public void Initialize()
        {
            Directory.CreateDirectory(Config.tempDirectoryName);
        }
        static public FileStream Create()
        {
            var dt = DateTime.Now;
            var date = dt.ToString("yyyy_MM_dd_HH_mm_ss");
            //Memo: 符号ビットを削除して、0 ~ MaxValue 24.9 日に1回ずつサイクルする正数値を生成します。
            int tick = Environment.TickCount & Int32.MaxValue;
            var basename = String.Format("{0}_{1}",date,tick);
            var tempFilename=Path.Combine(Config.tempDirectoryName, basename);
            //return new FileStream(tempFilename,FileMode.Create,FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose | FileOptions.SequentialScan);
            return File.Create(tempFilename, 4096, FileOptions.SequentialScan);
        }
    }
}
