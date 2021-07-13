using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{    
    [ComVisible(true)]
    [Guid("0B0A4550-A71F-4142-A4EC-BC6DF50B9590")]
    public class Service
    {
        static DllAssemblyResolver dasmr_ = new DllAssemblyResolver();

        IHidemaruLspBackEndServer server_ = null;
        ILspClientLogger logger_ = null;        
        ITargetServer targetServer_=null;

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
                        targetServer_ = server_.CreateTargetServer();
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
                if (server_ != null)
                {
                    server_.Finalizer(targetServer_,reason);
                    server_ = null;
                }
                dasmr_ = null;
                logger_ = null;
                targetServer_ = null;
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
        public bool Start(string serverConfigFilename, string currentSourceCodeDirectory)
        {
            try
            {
                var options = Configuration.Eval(serverConfigFilename, currentSourceCodeDirectory);
                if (options == null)
                {
                    return false;
                }
                targetServer_.ServerName = options.ServerName;
                targetServer_.RootUri     = options.RootUri;
                return server_.Start(
                    targetServer_,
                    options.ExcutablePath, 
                    options.Arguments,
                    options.WorkspaceConfig,
                    currentSourceCodeDirectory);
            }
            catch (Exception e)
            {
                logger_.Error(e.ToString());
            }
            return false;
        }
        private DigOpenStatus DigOpen(string absFilename)
        {
            if (openedFile.Filename == absFilename)
            {
                return DigOpenStatus.AlreadyOpened;
            }            
            var text = Hidemaru.GetTotalTextUnicode();
            var contentsVersion   = 1;
            server_.DigOpen(targetServer_,absFilename,text, contentsVersion);

            var sourceUri = new Uri(absFilename);            
            openedFile.Filename         = absFilename;
            openedFile.Uri              = sourceUri;
            openedFile.ContentsVersion  = contentsVersion;
            openedFile.ContentsHash     = text.GetHashCode();            
            return DigOpenStatus.Opened;
        }
        private DigChangeStatus DigChange(string absFilename)
        {
            Debug.Assert(openedFile.Filename == absFilename);
            var text = Hidemaru.GetTotalTextUnicode();
            {
                var currentHash = text.GetHashCode();
                var prevHash    = openedFile.ContentsHash;
                if (currentHash == prevHash)
                {
                    return DigChangeStatus.NoChanged;
                }
            }
            ++openedFile.ContentsVersion;
            server_.DigChange(targetServer_, absFilename,text,openedFile.ContentsVersion);
            return DigChangeStatus.Changed;
        }
        public string Completion(string absFilename, long line, long column)
        {
            try
            {
                if (DigOpen(absFilename) == DigOpenStatus.AlreadyOpened)
                {
                    DigChange(absFilename);
                }            
                return server_.Completion(targetServer_, absFilename, line, column);
            }catch(Exception e)
            {
                logger_.Error(e.ToString());
            }
            return "";
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
