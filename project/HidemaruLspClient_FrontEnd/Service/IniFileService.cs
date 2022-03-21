using HidemaruLspClient_BackEndContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// Iniファイル
    /// </summary>
    class IniFileService
    {
        IniFileService(string iniFilename, ILspClientLogger logger)
        {
            iniReader_ = new IniFileNative(iniFilename);
            iniFileDirectory_ = Path.GetDirectoryName(iniFilename);
            logger_ = logger;
            updateCount_ = 0;

            watcher_ = new FileSystemWatcher();
            watcher_.Path = iniFileDirectory_;
            watcher_.Filter = Path.GetFileName(iniFilename);
            watcher_.NotifyFilter = NotifyFilters.LastWrite|NotifyFilters.Size;
            watcher_.IncludeSubdirectories = false;
            watcher_.Changed += Watcher__Changed;
            watcher_.EnableRaisingEvents = true;
        }
        void Watcher__Changed(object sender, FileSystemEventArgs e)
        {
            updateCount_++;
            /*Debug.WriteLine("==== change ====");
            Debug.WriteLine(e.FullPath);
            Debug.WriteLine(e.Name);
            Debug.WriteLine(e.ChangeType.ToString());*/
            //iniReader_=new IniFileNative()
            //throw new NotImplementedException();
        }

        public static IniFileService Create(string iniFilename, ILspClientLogger logger) {
            if (!File.Exists(iniFilename))
            {
                logger?.Error(string.Format(".Ini file not found. iniFilename={0}", iniFilename));
                return null;
            }
            return new IniFileService(iniFilename, logger);
        }        
        /// <summary>
        /// iniファイルからサーバ設定ファイルを見付ける
        /// </summary>
        /// <param name="fileExtension">ファイル拡張子(".c", ".cpp" ...)</param>
        /// <returns>サーバ設定ファイル（絶対パス）、または空文字</returns>
        public string FindServerConfig(string fileExtension)
        {
            try
            {
                var path = iniReader_.Read(fileExtension, "ServerConfig");
                if (path == "")
                {
                    logger_?.Info(string.Format($"{fileExtension} not found in .ini file."));
                    return "";
                }
                if (Path.IsPathRooted(path))
                {
                    return path;
                }
                //iniFileからの相対パス→絶対パス
                var absFileName = Path.Combine(iniFileDirectory_, path);
                return absFileName;
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }
            return "";
        }
        
        public int UpdateCount { get { return updateCount_; } }
        int updateCount_;

        string iniFileDirectory_;
        ILspClientLogger logger_;
        IniFileNative iniReader_;
        FileSystemWatcher watcher_;
    }
}
