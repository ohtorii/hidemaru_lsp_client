﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{    
    /// <summary>
    /// 秀丸エディタへ公開するクラス
    /// </summary>
    [ComVisible(true)]
    [Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9590")]
    public class Service
    {
        static DllAssemblyResolver dasmr_ = new DllAssemblyResolver();

        IHidemaruLspBackEndServer server_ = null;
        IWorker worker_ = null;
        ILspClientLogger logger_ = null;                

        class Document
        {
            public string Filename { get; set; } = "";
            public Uri Uri { get; set; } = null;
            public int ContentsVersion { get; set; } = 1;
            public int ContentsHash { get; set; } = 0;
        }
        Document openedFile = new Document();

        enum DigOpenStatus
        {
            /// <summary>
            /// ファイルを開いた
            /// </summary>
            Opened,
            /// <summary>
            /// ファイールはオープン済み
            /// </summary>
            AlreadyOpened,
            /// <summary>
            /// 失敗
            /// </summary>
            Failed,
        }
        enum DigChangeStatus
        {
            /// <summary>
            /// 変更あり
            /// </summary>
            Changed,
            /// <summary>
            /// 変更無し
            /// </summary>
            NoChanged,
            /// <summary>
            /// 失敗
            /// </summary>
            Failed,
        }

        public bool Initialize(string logFilename)
        {
            try
            {
                Hidemaru.Initialize();

                if (server_ == null)
                {
                    //事前にBackEndをspawnしたほうが良いと思う
                    object obj;
                    int hr = Ole32.CoCreateInstance(LspContract.Constants.ServerClassGuid, IntPtr.Zero, Ole32.CLSCTX_LOCAL_SERVER, typeof(IHidemaruLspBackEndServer).GUID, out obj);
                    if (hr < 0)
                    {
                        Marshal.ThrowExceptionForHR(hr);
                    }
                    server_ = (IHidemaruLspBackEndServer)obj;
                    var ret = server_.Initialize(logFilename);
                    if (ret)
                    {                     
                        logger_     = server_.GetLogger();
                        Configuration.Initialize(logger_);
                    }
                    else
                    {
                        server_ = null;
                    }
                    return ret;
                }
                return true;
            }
            catch(Exception e)
            {
                server_ = null;
            }
            return false;
        }
        /// <summary>
        /// テスト用のメソッド
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Add(int x, int y)
        {
            try
            {
                return server_.Add(x, y);
            }
            catch (System.Exception )
            {
                return -1;
            }
        }
        public void Finalizer(int reason)
        {
            try
            {
                if ((server_ != null) && (worker_ != null))
                {
                    server_.DestroyWorker(worker_);
                }
                worker_ = null;
                server_ = null;
                dasmr_ = null;
                logger_ = null;                
            }
            catch (Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return;
        }
        public bool CreateWorker(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            try
            {
                Debug.Assert(worker_ == null);

                var options = Configuration.Eval(serverConfigFilename, currentSourceCodeDirectory);
                if (options == null)
                {
                    return false;
                }                
                worker_=server_.CreateWorker(
                            options.ServerName,
                            options.ExcutablePath, 
                            options.Arguments,
                            options.RootUri,
                            options.WorkspaceConfig);
                if (worker_ == null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return false;
        }
        private DigOpenStatus DigOpen(string absFilename)
        {
            try
            {
                Debug.Assert(worker_ != null);

                if (openedFile.Filename == absFilename)
                {
                    return DigOpenStatus.AlreadyOpened;
                }
                var text = Hidemaru.GetTotalTextUnicode();
                var contentsVersion = 1;
                worker_.DigOpen(absFilename, text, contentsVersion);

                var sourceUri = new Uri(absFilename);
                openedFile.Filename = absFilename;
                openedFile.Uri = sourceUri;
                openedFile.ContentsVersion = contentsVersion;
                openedFile.ContentsHash = text.GetHashCode();
                return DigOpenStatus.Opened;
            }
            catch (Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return DigOpenStatus.Failed;
        }
        private DigChangeStatus DigChange(string absFilename)
        {
            try
            {
                Debug.Assert(worker_ != null);
                Debug.Assert(openedFile.Filename == absFilename);
                var text = Hidemaru.GetTotalTextUnicode();
                {
                    var currentHash = text.GetHashCode();
                    var prevHash = openedFile.ContentsHash;
                    if (currentHash == prevHash)
                    {
                        return DigChangeStatus.NoChanged;
                    }
                }
                ++openedFile.ContentsVersion;
                worker_.DigChange(absFilename, text, openedFile.ContentsVersion);
                return DigChangeStatus.Changed;
            }catch(Exception e)
            {
                if (logger_ != null)
                {
                    logger_.Error(e.ToString());
                }
            }
            return DigChangeStatus.Failed;
        }
        /// <summary>
        /// textDocument/completion
        /// </summary>
        /// <param name="absFilename"></param>
        /// <param name="line"></param>
        /// <param name="column"></param>
        /// <returns>辞書ファイル名</returns>
        public string Completion(string absFilename, long line, long column)
        {
            Debug.Assert(worker_ != null);
            try
            {
                switch (DigOpen(absFilename))
                {
                    case DigOpenStatus.Opened:
                        //pass
                        break;
                    case DigOpenStatus.AlreadyOpened:
                        if (DigChange(absFilename) == DigChangeStatus.Failed)
                        {
                            return "";
                        }
                        break;
                    case DigOpenStatus.Failed:
                        return "";
                    default:
                        return "";
                }
                return worker_.Completion(absFilename, line, column);
            }catch(Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
        }
        /// <summary>
        /// textDocument/publishDiagnostics
        /// </summary>
        /// <param name="absFilename"></param>
        /// <returns>改行区切りの文字列</returns>
        public string Diagnostics(string absFilename)
        {
            Debug.Assert(worker_ != null);
            try
            {
                var notification = worker_.Diagnostics(absFilename);
                if (notification == null)
                {
                    return "";
                }
                var length = notification.getDiagnosticsLength();
                for (long i = 0; i < length; ++i)
                {
                    var diagnostic=notification.getDiagnostics(i);
                    //diagnostic.range.
                }
            }catch(Exception e)
            {
                return "";
            }
        }




        private class Ole32
        {
            // https://docs.microsoft.com/windows/win32/api/wtypesbase/ne-wtypesbase-clsctx
            public const int CLSCTX_LOCAL_SERVER = 0x4;

            // https://docs.microsoft.com/windows/win32/api/combaseapi/nf-combaseapi-cocreateinstance
            [DllImport(nameof(Ole32))]
            public static extern int CoCreateInstance(
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid rclsid,
                IntPtr pUnkOuter,
                uint dwClsContext,
                [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid,
                [MarshalAs(UnmanagedType.IUnknown)] out object ppv);
        }

    }

    
}
