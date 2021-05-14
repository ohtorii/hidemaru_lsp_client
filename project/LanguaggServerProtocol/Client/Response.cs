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
        const string HeaderContentLength_ = "Content-Length: ";
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
                        if (OnSkipSeparator() == false)
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
            lock (bufferStreamUTF8)
            {
                /* "Content-Length: "を見付ける。
                 * (Ex)
                 * bufferStream=Content-Length: 
                 */
                if (bufferStreamUTF8.Count < HeaderContentLengthLength_)
                {
                    return false;
                }
                if (ByteListUtil.StartsWith(bufferStreamUTF8, HeaderContentLength_))
                {
                    return true;
                }
                /*Content-Length:以外が渡されたため、Content-Length:まで読み飛ばす。
                */
                var headerIndex = ByteListUtil.StrStr(bufferStreamUTF8, HeaderContentLength_);
                if (headerIndex == -1)
                {
                    return false;
                }
                bufferStreamUTF8.RemoveRange(0, headerIndex);
                var uni = Encoding.UTF8.GetString(bufferStreamUTF8.ToArray());
            }
            return true;
        }
        bool OnParseContextLength()
        {
            lock (bufferStreamUTF8)
            {
                var tail = bufferStreamUTF8.IndexOf(Convert.ToByte('\r'), HeaderContentLengthLength_);
                if (tail < 0)
                {
                    tail = bufferStreamUTF8.IndexOf(Convert.ToByte('\n'), HeaderContentLengthLength_);
                    if (tail < 0)
                    {
                        return false;
                    }
                }
                {
                    /*(Ex)
                    bufferStream=Content-Length: 52\n\n{"method":"initialized",...
                                                   ^
                                                   |
                                               tail+
                    contentLength=52;
                    */
                    int numericalLen = tail - HeaderContentLengthLength_;
                    var byteArray = bufferStreamUTF8.GetRange(HeaderContentLengthLength_, numericalLen);
                    var unicodeLen = Encoding.UTF8.GetString(byteArray.ToArray());
                    var value = string.Join("", unicodeLen);
                    contentLength = int.Parse(value);
                    //var len = bufferStreamUTF8.Substring(HeaderContentLengthLength_, numericalLen);
                    //contentLength = int.Parse(len);
                }

                /*(Ex)
                (Before) bufferStream=Content-Length: 52\n\n{"method":"initialized",...
                (After)  bufferStream=\n\n{"method":"initialized",...
                */
                bufferStreamUTF8.RemoveRange(0, tail + 1);
            }
            return true;
        }

        bool OnSkipSeparator()
        {
            lock (bufferStreamUTF8)
            {
                /*(Ex)
                 * (Before) bufferStream=\n\n{"method":"initialized",...
                 */
                if (bufferStreamUTF8.Count < 2)
                {
                    return false;
                }
                if ((bufferStreamUTF8[0] == '\n') || (bufferStreamUTF8[0] == '\r'))
                {
                    ByteListUtil.TrimStart(bufferStreamUTF8, Convert.ToByte('\n'), Convert.ToByte('\r'));
                    //bufferStreamUTF8 = bufferStreamUTF8.TrimStart(new char[] { '\n', '\r' });
                }
                if (bufferStreamUTF8.Count == 0)
                {
                    return false;
                }
                System.Diagnostics.Debug.Assert((bufferStreamUTF8[0] != '\n') && (bufferStreamUTF8[0] != '\r'));

                /*(Ex)
                 * (After) bufferStream={"method":"initialized",...
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
                    debugLogger.Log(jsonDataUnicode);
                    receiver = (JObject)JsonConvert.DeserializeObject(jsonDataUnicode);
                }
                catch (JsonReaderException e)
                {
                    //Console.WriteLine(e);
                    throw;
                }

                /*
                 * (Before) bufferStream={"method":"initialized",...}Content-Length: 128\n\n{
                 * (After)  bufferStream=Content-Length: 128\n\n{
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
                var param = (JObject)notification.@params;

                switch (notification.method)
                {
                    case "window/logMessage":
                        if (this.param.OnWindowLogMessage != null)
                        {
                            var jVal = (JValue)param["type"];
                            var jMes = (JValue)param["message"];
                            var val = (MessageType)Enum.ToObject(typeof(MessageType), jVal.Value);
                            var mes = (string)jMes.Value;
                            this.param.OnWindowLogMessage(new LogMessageParams { type = val, message = mes });
                        }
                        break;
                    case "window/showMessage":
                        var showMessage = (ShowMessageParams)notification.@params;
                        break;
                    default:
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
             * bufferStream={"method":"initialized",...}Content-Length: 128\n\n{
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
            public static bool IsSame(List<byte> buf, int bufStartIndex, byte[] str)
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
            public static int StrStr(List<byte> buf, string str)
            {
                const int NotFound = -1;

                if (buf.Count < str.Length)
                {
                    return NotFound;
                }

                var byteStr = Encoding.UTF8.GetBytes(str);
                var bufCount = (buf.Count - str.Length) + 1;
                for (int i = 0; i < bufCount; ++i)
                {
                    if (IsSame(buf, i, byteStr))
                    {
                        return i;
                    }
                }
                return NotFound;
            }
        }
    }
}
