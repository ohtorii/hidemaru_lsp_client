using HidemaruLspClient_BackEndContract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    class IniFileService
    {
        public static IniFileService Create(string iniFilename, ILspClientLogger logger) {
            if (!File.Exists(iniFilename))
            {
                logger?.Error(string.Format(".Ini file not found. iniFilename={0}", iniFilename));
                return null;
            }
            return new IniFileService(iniFilename, logger);
        }
        IniFileService(string iniFilename, ILspClientLogger logger)
        {
            iniReader_          = new IniFileNative(iniFilename);
            iniFileDirectory_   = Path.GetDirectoryName(iniReader_.Filename);
            logger_             = logger;
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

        string iniFileDirectory_;
        ILspClientLogger logger_;
        IniFileNative iniReader_;
    }
}
