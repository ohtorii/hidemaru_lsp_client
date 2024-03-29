﻿using HidemaruLspClient_BackEndContract;
using HidemaruLspClient_FrontEnd.Native;
using System;
using System.IO;

namespace HidemaruLspClient_FrontEnd.Facility

{
    /// <summary>
    /// Iniファイル
    /// </summary>
    class IniFileService
    {
        //Iniファイル変更後イベント
        public event EventHandler OnFileChanged;

        IniFileService(string iniFilename)
        {
            iniReader_ = new IniFileNative(iniFilename);
            iniFileDirectory_ = Path.GetDirectoryName(iniFilename);
            logger_ = null;
            updateCount_ = 0;

            watcher_ = new FileSystemWatcher();
            watcher_.Path = iniFileDirectory_;
            watcher_.Filter = Path.GetFileName(iniFilename);
            watcher_.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher_.IncludeSubdirectories = false;
            watcher_.Changed += Watcher__Changed;
            watcher_.EnableRaisingEvents = true;
        }
        void Watcher__Changed(object sender, FileSystemEventArgs e)
        {
            updateCount_++;
            if (OnFileChanged != null)
            {
                OnFileChanged(this, EventArgs.Empty);
            }
        }

        public static IniFileService Create(string iniFilename)
        {
            if (!File.Exists(iniFilename))
            {
                return null;
            }
            return new IniFileService(iniFilename);
        }

        public void SetLogger(ILspClientLogger logger)
        {
            logger_ = logger;
        }
        /// <summary>
        /// [SearchServerIni]セクションから存在するiniファイルを見付ける
        /// </summary>
        /// <returns></returns>
        string SearchExistServerIni()
        {
            const string sectionName = "SearchServerIni";
            const int keyFirst = 1;
            const int keyLast = 5;

            for (int i = keyFirst; i < keyLast; ++i)
            {
                var keyName = $"FileName{i}";
                var keyValue = iniReader_.Read(keyName, sectionName);
                if (string.IsNullOrEmpty(keyValue))
                {
                    continue;
                }

                string absFileName;
                var expandedFileName = Environment.ExpandEnvironmentVariables(keyValue);
                if (Path.IsPathRooted(expandedFileName))
                {
                    absFileName = expandedFileName;
                }
                else
                {
                    //iniFileからの相対パス→絶対パス
                    absFileName = Path.GetFullPath(Path.Combine(iniFileDirectory_, expandedFileName));
                }
                if (File.Exists(absFileName))
                {
                    logger_?.Debug($"Ini file exist. sectionName={sectionName} / keyName={keyName} / keyValue={keyValue} / absFileName={absFileName}");
                    return absFileName;
                }
                else
                {
                    logger_?.Debug($"Ini file not exist. sectionName={sectionName} / keyName={keyName} / keyValue={keyValue} / absFileName={absFileName}");
                }
            }
            return null;
        }
        /// <summary>
        /// iniファイルからサーバ設定ファイルを見付ける
        /// </summary>
        /// <param name="fileExtension">ファイル拡張子(".c", ".cpp" ...)</param>
        /// <returns>サーバ設定ファイル（絶対パス）、または空文字</returns>
        public string FindServerConfig(string fileExtension)
        {
            string absFileName = null;
            try
            {
                var serverIniFilename = SearchExistServerIni();
                if (string.IsNullOrEmpty(serverIniFilename))
                {
                    return null;
                }

                var serverIniReader = new IniFileNative(serverIniFilename);
                const string sectionName = "ServerConfig";
                var path = serverIniReader.Read(fileExtension, sectionName);
                if (string.IsNullOrEmpty(path))
                {
                    logger_?.Info($"Not found key in ini file. section={sectionName} / key={fileExtension} / serverIniFilename={serverIniFilename}");
                    return null;
                }
                if (Path.IsPathRooted(path))
                {
                    return path;
                }
                //iniFileからの相対パス→絶対パス
                var serverIniDirectoryName = Path.GetDirectoryName(serverIniFilename);
                absFileName = Path.GetFullPath(Path.Combine(serverIniDirectoryName, path));
                return absFileName;
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }
            finally
            {
                logger_?.Info($"FindServerConfig return. \"{fileExtension}\" -> \"{absFileName}\"");
            }
            return null;
        }
        /// <summary>
        /// MacroImprovementProgram -> SendCrashReport の値(bool)を読み取る
        /// </summary>
        /// <returns></returns>
        public bool ReadEnableCrashReport()
        {
            string crashReport = "";
            try
            {
                crashReport = iniReader_.Read("SendCrashReport", "MacroImprovementProgram");
                if (String.IsNullOrEmpty(crashReport) || String.IsNullOrWhiteSpace(crashReport))
                {
                    return false;
                }
                return StringToBoolean(crashReport);
            }
            catch (Exception e)
            {
                logger_?.Error(e.ToString());
            }
            return false;
        }

        static bool StringToBoolean(string value, bool @default = false)
        {
            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "t":
                    return true;
                case "1":
                    return true;
                case "0":
                    return false;
                case "false":
                    return false;
                case "f":
                    return false;
                default:
                    return @default;
            }
        }
        public int UpdateCount { get { return updateCount_; } }
        int updateCount_;

        string iniFileDirectory_;
        ILspClientLogger logger_;
        IniFileNative iniReader_;
        FileSystemWatcher watcher_;
    }
}
