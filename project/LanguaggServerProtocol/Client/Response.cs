using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


namespace LSP.Client
{
    class Response
    {
        public class InitializeParameter
        {
            /// <summary>
            /// method: ‘window/logMessage’
            /// </summary>
            public Action<LogMessageParams> OnWindowLogMessage = null;
            /// <summary>
            /// method: ‘window/showMessage’
            /// </summary>
            public Action<ShowMessageParams> OnWindowShowMessage = null;
            public Action<ResponseMessage> OnResponseError = null;

            public string logFileName;
        }
        InitializeParameter param = null;


        enum Mode
        {
            FindContextLength,
            ParseContextLength,
            SkipSeparator,
            ParseContext,
        }
        Mode mode = Mode.FindContextLength;
        Dictionary<int, Action<JToken>> responseCallback =new Dictionary<int, Action<JToken>>();

        CancellationToken cancelToken_;
        
        int contentLength = -1;
        const string HeaderContentLength_ = "Content-Length:";
        static int HeaderContentLengthLength_ = HeaderContentLength_.Length;

        Logger debugLogger;
        List<byte> bufferStreamUTF8 = new List<byte>();
        

        public Response(InitializeParameter param, CancellationToken token)
		{
            this.param = param;
            this.cancelToken_ = token;
            this.debugLogger = new Logger(param.logFileName);
        }
        public bool StoreBuffer(byte[] streamString)
		{
            if (streamString.Length == 0)
            {
                return false;
            }
            if (debugLogger != null)
            {
                var jsonDataUnicode = Encoding.UTF8.GetString(streamString);
                debugLogger.Log(jsonDataUnicode);
            }

            lock (bufferStreamUTF8)
            {
                bufferStreamUTF8.AddRange(streamString);
                return true;
            }
        }        
        public void StoreJob(int id, Action<JToken> callback)
		{
            lock (responseCallback)
            {
                Debug.Assert(responseCallback.ContainsKey(id) == false);
                responseCallback.Add(id, callback);
            }
        }
        public void Parse()
        {
            //Todo: EventWaitHandle or Task で書き直す

            int sleepTime = 10;
            while (cancelToken_.IsCancellationRequested == false)
            {
                switch (mode)
                {
                    case Mode.FindContextLength:
                        if (OnFindContextLength() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode = Mode.ParseContextLength;
                        break;
                    case Mode.ParseContextLength:
                        if (OnParseContextLength() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode = Mode.SkipSeparator;
                        break;
                    case Mode.SkipSeparator:
                        if (OnSkipContextHeader() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode = Mode.ParseContext;
                        break;
                    case Mode.ParseContext:
                        if (OnParseContext() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode = Mode.FindContextLength;
                        Restart();
                        break;
                }
            }
        }

        void Restart()
        {
            contentLength = 0;
        }
        bool OnFindContextLength()
        {
            /*Memo
             * 以下2通りの記述方法あり。
             * "Content-Length:123"
             * "Content-Length: 123"
             */
            lock (bufferStreamUTF8)
            {
                //var jsonDataUnicode = Encoding.UTF8.GetString(bufferStreamUTF8.ToArray());

                /* "Content-Length:"を見付ける。
                 */
                if (bufferStreamUTF8.Count < HeaderContentLengthLength_)
                {
                    return false;
                }
                if (ByteListUtil.StartsWith(bufferStreamUTF8, HeaderContentLength_))
                {
                    return true;
                }
#if false
                /*Content-Length:以外が渡されたため、Content-Length:まで読み飛ばす。
                */
                var headerIndex = ByteListUtil.StrStr(bufferStreamUTF8, HeaderContentLength_);
                if (headerIndex == -1)
                {
                    return false;
                }
                bufferStreamUTF8.RemoveRange(0, headerIndex);
                var uni = Encoding.UTF8.GetString(bufferStreamUTF8.ToArray());
#endif
            }
            return false;
        }
        bool OnParseContextLength()
        {
            lock (bufferStreamUTF8)
            {
                /*bufferStreamUTF8=Content-Length: 52\r\nContent-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                 *                                   ^
                 *                                   |
                 *                                   tail
                */
                var tail = ByteListUtil.StrStr(bufferStreamUTF8, "\r\n");
                if (tail == -1)
                {
                    return false;
                }

                {
                    
                    int numericalLen = tail - HeaderContentLengthLength_;
                    var byteArray = bufferStreamUTF8.GetRange(HeaderContentLengthLength_, numericalLen);
                    var unicodeLen = Encoding.UTF8.GetString(byteArray.ToArray());
                    var value = string.Join("", unicodeLen);
                    contentLength = int.Parse(value);
                }

                /*(Ex)
                (Before) bufferStreamUTF8=Content-Length: 52\nContent-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                (After)  bufferStreamUTF8=Content-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                */
                bufferStreamUTF8.RemoveRange(0, tail + 2);//2="\r\n"
            }
            return true;
        }

        bool OnSkipContextHeader()
        {
            lock (bufferStreamUTF8)
            {
                /*
                 * (Before) bufferStreamUTF8=Content-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                 *              or
                 *          bufferStreamUTF8=\r\n\r\n{"method":"initialized",...
                 */


                /* bufferStreamUTF8=Content-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                 *                                                                        ^       ^
                 *                                                                        |       |
                 *                                                                        |       separatorEndIndex;
                 *                                                                        separatorFirstIndex;
                 *              or
                 *
                 *bufferStreamUTF8=\r\n\r\n{"method":"initialized",...
                 *                 ^       ^
                 *                 |       |
                 *                 |       separatorEndIndex;
                 *                 separatorFirstIndex;
                 */
                var separatorFirstIndex = ByteListUtil.StrStr(bufferStreamUTF8, "\r\n");
				if (separatorFirstIndex == -1)
				{
                    return false;
				}
                var separatorEndIndex =ByteListUtil.StrSpn(bufferStreamUTF8, separatorFirstIndex, "\r\n");
				if (separatorEndIndex == -1)
				{
                    return false;
				}                
                bufferStreamUTF8.RemoveRange(0, separatorEndIndex);                
                Debug.Assert(bufferStreamUTF8[0]== Convert.ToByte('{'));
                /*(After) bufferStreamUTF8={"method":"initialized",...
                 */
            }
            return true;
        }
        bool OnParseContext()
        {
            JObject receiver = null;

            lock (bufferStreamUTF8)
            {
                System.Diagnostics.Debug.Assert(0 < contentLength);
                if (bufferStreamUTF8.Count < contentLength)
                {
                    return false;
                }
#if false
            int pairKakkoIndex = 0;
            if(FindPairKakko(out pairKakkoIndex) == false)
			{
                return false;
			}
            if(contentLength != (pairKakkoIndex + 1))
			{
                var bufferStreamUnicode= Encoding.UTF8.GetString(this.bufferStreamUTF8.ToArray());
                var prevJsonUnicode = this.prevJson;
                System.Diagnostics.Debug.Assert(false);
            }
            contentLength = pairKakkoIndex + 1;
#endif
                var jsonDataUTF8 = bufferStreamUTF8.GetRange(0, contentLength);
                var jsonDataUnicode = Encoding.UTF8.GetString(jsonDataUTF8.ToArray());
                try
                {
                    receiver = (JObject)JsonConvert.DeserializeObject(jsonDataUnicode);
                }
                catch (JsonReaderException e)
                {
                    //Console.WriteLine(e);
                    throw;
                }

                /*
                 * (Before) bufferStreamUTF8={"method":"initialized",...}Content-Length: 128\n\n{
                 * (After)  bufferStreamUTF8=Content-Length: 128\n\n{
                 */
                bufferStreamUTF8.RemoveRange(0, contentLength);
            }
            
            if (receiver.ContainsKey("id"))
            {
                Action<JToken> callback;
                {
                    var id = receiver["id"].ToObject<int>();
                    try
                    {
                        lock (responseCallback)
                        {
                            callback = responseCallback[id];
                            responseCallback.Remove(id);
                        }
                    }
                    catch (Exception)
                    {
                        //対応するid無し。無視する。
                        //return true;
                        throw;
                    }
                }
                
                var response = receiver.ToObject<ResponseMessage>();
                if (response.error == null)
                {
                    callback((JToken)response.result);
				}
				else
				{
                    param.OnResponseError(response);
				}
                /*if (((JObject)response.result).ContainsKey("capabilities"))
                {
                    var result = ((JObject)response.result).ToObject<InitializeResult>();
                    var method= items.Item1;
                    items.Item2(result);
                }
                else
                {
                    throw new NotImplementedException();
                }*/

                return true;
            }

            if (receiver.ContainsKey("method"))
            {
                var notification = receiver.ToObject<NotificationMessage>();                
                switch (notification.method)
                {
                    case "window/logMessage":
                        if (this.param.OnWindowLogMessage != null)
                        {
                            var _param = (JObject)notification.@params;
                            var jVal = (JValue)_param["type"];
                            var jMes = (JValue)_param["message"];
                            var val = (MessageType)Enum.ToObject(typeof(MessageType), jVal.Value);
                            var mes = (string)jMes.Value;
                            this.param.OnWindowLogMessage(new LogMessageParams { type = val, message = mes });
                        }
                        break;
                    case "window/showMessage":
                        var showMessage = (ShowMessageParams)notification.@params;
                        break;
                    default:
                        Console.WriteLine(String.Format("[{0}]{1}", notification.method, notification.@params));
                        //pass
                        break;
                }
            }
            else
            {
                var oops = 0;
            }
            return true;
        }

        bool FindPairKakko(out int outPairKakkoIndex)
        {
            /*(Ex)
             * bufferStreamUTF8={"method":"initialized",...}Content-Length: 128\n\n{
             *                                         ^
             *                                         |
             *                     outPairKakkoIndex---+
             */
            outPairKakkoIndex = -1;

            System.Diagnostics.Debug.Assert(bufferStreamUTF8[0] == Convert.ToByte('{'));

            int index = 0;
            int counter = 0;

            foreach (var c in bufferStreamUTF8)
            {
                if (c == Convert.ToByte('{'))
                {
                    ++counter;
                }
                else if (c == Convert.ToByte('}'))
                {
                    --counter;
                }

                if (counter == 0)
                {
                    outPairKakkoIndex = index;
                    return true;
                }

                ++index;
            }

            return false;
        }

        class Logger
        {
            string logFileName_;
            bool firstWrite_;
            public Logger(string logFilename)
			{
                this.logFileName_ = logFilename;
                this.firstWrite_ = true;
            }
            public void Log(string str)
            {
                if ((logFileName_ == null) || (logFileName_.Length == 0))
                {
                    return;
                }
                if (firstWrite_)
                {
                    CreateDirectory();
                    File.WriteAllText(logFileName_, str);
                    firstWrite_ = false;
                }
                else
                {
                    File.AppendAllText(logFileName_, str);
                }
            }
            void CreateDirectory()
			{
                var dirName = System.IO.Path.GetDirectoryName(logFileName_);
                Directory.CreateDirectory(dirName);
            }
        }



        class ByteListUtil
        {
            public static bool StartsWith(List<byte> buf, string str)
            {
                if (buf.Count < str.Length)
                {
                    return false;
                }
                int i = 0;
                foreach (var c in str)
                {
                    var cc = Convert.ToByte(c);
                    if (buf[i] != cc)
                    {
                        return false;
                    }
                    ++i;
                }
                return true;
            }
            public static void TrimStart(List<byte> buf, params byte[] trimChars)
            {
                int count = 0;
                foreach (var c in buf)
                {
                    if (trimChars.Contains(c) == false)
                    {
                        break;
                    }
                    ++count;
                }
                buf.RemoveRange(0, count);
            }
            static bool IsSame(List<byte> buf, int bufStartIndex, byte[] str)
            {
                var i = bufStartIndex;
                foreach (var b in str)
                {
                    if (b != buf[i])
                    {
                        return false;
                    }
                    ++i;
                }
                return true;
            }
            /// <summary>
            /// 文字列で最初に見つかった検索文字列へのインデックスを返します。
            /// </summary>
            /// <param name="buf"></param>
            /// <param name="Search"></param>
            /// <returns></returns>
            public static int StrStr(List<byte> buf, string Search)
            {
                const int NotFound = -1;

                if (buf.Count < Search.Length)
                {
                    return NotFound;
                }

                var byteStr = Encoding.UTF8.GetBytes(Search);
                var bufCount = (buf.Count - Search.Length) + 1;
                for (int i = 0; i < bufCount; ++i)
                {
                    if (IsSame(buf, i, byteStr))
                    {
                        return i;
                    }
                }
                return NotFound;
            }
            /// <summary>
            /// 文字列の中で、文字セットに含まれない最初の文字へのインデックスを返します。
            /// </summary>
            /// <param name="buf"></param>
            /// <param name="bufCharSet"></param>
            /// <returns></returns>
            public static int StrSpn(List<byte> buf, string bufCharSet)
            {
                return StrSpn(buf,0,bufCharSet);
            }
            /// <summary>
            /// 文字列の中で、文字セットに含まれない最初の文字へのインデックスを返します。
            /// </summary>
            /// <param name="buf"></param>
            /// <param name="startIndex"></param>
            /// <param name="strCharSet"></param>
            /// <returns></returns>
            public static int StrSpn(List<byte> buf, int startIndex, string strCharSet)
			{
                const int NotFound = -1;
                var byteCharSet = Encoding.UTF8.GetBytes(strCharSet);
                var bufCount = (buf.Count - byteCharSet.Length) + 1;
                for (int i = startIndex; i < bufCount; ++i)
                {
					if (byteCharSet.Contains(buf[i]))
					{
                        continue;
					}
                    return i;
                }
                return NotFound;
            }
        }
    }
}
