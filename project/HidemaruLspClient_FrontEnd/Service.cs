using System;
using System.Collections.Generic;
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
        IHidemaruLspBackEndServer server_ = null;
        static DllAssemblyResolver dasmr = new DllAssemblyResolver();

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
                }
                return server_.Initialize(logFilename);
            }
            catch(Exception e)
            {
                //Todo: あとで実装
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
            catch (System.Exception e)
            {
                return -1;
            }
        }
        public void Finalizer(int reason)
        {
            try
            {
                server_.Finalizer(reason);
                server_ = null;
                dasmr = null;
            }
            catch (Exception e)
            {
                //Todo:あとで実装
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
                return server_.Start(
                    options.ExcutablePath, 
                    options.Arguments,
                    options.RootUri,
                    options.WorkspaceConfig,
                    currentSourceCodeDirectory);
            }
            catch (Exception e)
            {
                //logger.Error(e);
            }
            return false;
        }
        public string Completion(string absFilename, long line, long column)
        {
            try
            {
                var text = Hidemaru.GetTotalTextUnicode();
                return server_.Completion(absFilename, line, column, text);
            }catch(Exception e)
            {
                //Todo: あとで実装
                //log
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
