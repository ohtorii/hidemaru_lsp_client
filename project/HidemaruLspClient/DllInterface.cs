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
            try
            {
                var intLine = line.ToInt32();
                var intColumn = column.ToInt32();
                if (intLine < 0)
                {
                    return False;
                }
                if (intColumn < 0)
                {
                    return False;
                }
                Holder.Completion(Marshal.PtrToStringAuto(absFilename), (uint)intLine, (uint)intColumn);
                /*if ()
                {
                    return True;
                }*/
                return True;
            }
            catch (Exception)
            {
                //pass
            }
            return False;
		}
        
	}
}
