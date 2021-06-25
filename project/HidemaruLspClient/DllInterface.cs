using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient
{
    public class DllInterface
    {
        static readonly IntPtr True  = (IntPtr)1;   //1==True
		static readonly IntPtr False = (IntPtr)0;   //0==False
        static readonly IntPtr EmptyString = Marshal.StringToHGlobalUni("");
        static DllAssemblyResolver  dasmr            = new DllAssemblyResolver();
        static LspClientLogger      lspClientLogger  = new LspClientLogger(Config.logFileName);

        static DllInterface()
        {
            Holder.Initialized(lspClientLogger);
        }
        static IntPtr BoolToIntptr(bool value)
        {
            if (value)
            {
                return True;
            }
            return False;
        }
        [DllExport]
		public static IntPtr Start(IntPtr serverConfigFilename, IntPtr currentSourceCodeDirectory)
        {
            var logger = LogManager.GetCurrentClassLogger();

            logger.Trace("Start");
            try
            {
                var ret = Holder.Start(Marshal.PtrToStringAuto(serverConfigFilename), Marshal.PtrToStringAuto(currentSourceCodeDirectory));
                logger.Trace("Result={0}",ret);
                return BoolToIntptr(ret);
            }
            catch (Exception e)
            {
                //pass
                logger.Error(e);
            }
            return False;
		}
        /*[DllExport]
        public static IntPtr Stop()
        {

        }*/
#if false
        [DllExport]
		public static IntPtr DigOpen(IntPtr absFilename)
        {
            try{
                if (Holder.DigOpen(Marshal.PtrToStringAuto(absFilename)))
                {
                    return True;
                }
            }catch(Exception){
                //pass
            }
            return False;
        }
#endif
        [DllExport]
		public static IntPtr Completion(IntPtr absFilename, IntPtr line, IntPtr column)
		{
            var logger = LogManager.GetCurrentClassLogger();
            logger.Trace("Completion");
            try
            {
                var intLine = line.ToInt32();
                var intColumn = column.ToInt32();
                if (intLine < 0)
                {
                    return EmptyString;
                }
                if (intColumn < 0)
                {
                    return EmptyString;
                }
                var fileName = Holder.Completion(Marshal.PtrToStringAuto(absFilename), (uint)intLine, (uint)intColumn);
                if (fileName.Length == 0)
                {
                    return EmptyString;
                }
                //Todo: メモリを解放する
                var rawPtr = Marshal.StringToHGlobalUni(fileName);
                return rawPtr;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return EmptyString;
		}

        [DllExport]
        public static IntPtr DllDetachFunc_After_Hm866(IntPtr n)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                logger.Trace("DllDetachFunc_After_Hm866 (n={0})", n);
                Holder.Destroy();
            }catch(Exception e)
            {
                logger.Error(e);
            }
            return True;
        }
    }
}
