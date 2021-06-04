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
    class Protocol
    {
        public class InitializeParameter
        {
            /// <summary>
            /// method: ‘window/logMessage’
            /// </summary>
            public Action<LogMessageParams> OnWindowLogMessage { get; set; }
            /// <summary>
            /// method: ‘window/showMessage’
            /// </summary>
            public Action<ShowMessageParams> OnWindowShowMessage { get; set; }

            public Action<ResponseMessage> OnResponseError { get; set; }
            /// <summary>
            /// method: 'workspace/configuration'
            /// </summary>
            public Action<int,ConfigurationParams> OnWorkspaceConfiguration { get; set; }
            /// <summary>
            /// method: 'client/registerCapability'
            /// </summary>
            public Action<int, RegistrationParams> OnClientRegisterCapability { get; set; }
            public Action<int, WorkDoneProgressCreateParams> OnWindowWorkDoneProgressCreate { get; set; }
            /// <summary>
            /// method: ‘$/progress
            /// </summary>
            public Action<ProgressParams> OnProgress { get; set; }
        }
        InitializeParameter param_ = null;


        enum Mode
        {
            FindContextLength,
            ParseContextLength,
            SkipSeparator,
            ParseContext,
        }
        Mode mode_ = Mode.FindContextLength;
        Dictionary<int, Action<JToken>> responseCallback_ =new Dictionary<int, Action<JToken>>();

        CancellationToken cancelToken_;
        
        int contentLength = -1;
        const string HeaderContentLength_ = "Content-Length:";
        static int HeaderContentLengthLength_ = HeaderContentLength_.Length;
        
        List<byte> bufferStreamUTF8_ = new List<byte>();
        

        public Protocol(InitializeParameter param, CancellationToken token)
		{
            this.param_ = param;
            this.cancelToken_ = token;
        }
        public bool StoreBuffer(byte[] streamString)
		{
            if (streamString.Length == 0)
            {
                return false;
            }                        

            lock (bufferStreamUTF8_)
            {
                bufferStreamUTF8_.AddRange(streamString);
                return true;
            }
        }        
        public void StoreJob(int id, Action<JToken> callback)
		{
            lock (responseCallback_)
            {
                Debug.Assert(responseCallback_.ContainsKey(id) == false);
                responseCallback_.Add(id, callback);
            }
        }
        public void Parse()
        {
            //Todo: EventWaitHandle or Task で書き直す

            int sleepTime = 10;
            while (cancelToken_.IsCancellationRequested == false)
            {
                switch (mode_)
                {
                    case Mode.FindContextLength:
                        if (OnFindContextLength() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode_ = Mode.ParseContextLength;
                        break;
                    case Mode.ParseContextLength:
                        if (OnParseContextLength() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode_ = Mode.SkipSeparator;
                        break;
                    case Mode.SkipSeparator:
                        if (OnSkipContextHeader() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode_ = Mode.ParseContext;
                        break;
                    case Mode.ParseContext:
                        if (OnParseContext() == false)
                        {
                            Thread.Sleep(sleepTime);
                            break;
                        }
                        mode_ = Mode.FindContextLength;
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
            lock (bufferStreamUTF8_)
            {
                //var jsonDataUnicode = Encoding.UTF8.GetString(bufferStreamUTF8.ToArray());

                /* "Content-Length:"を見付ける。
                 */
                if (bufferStreamUTF8_.Count < HeaderContentLengthLength_)
                {
                    return false;
                }
                if (ByteListUtil.StartsWith(bufferStreamUTF8_, HeaderContentLength_))
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
            lock (bufferStreamUTF8_)
            {
                /*bufferStreamUTF8=Content-Length: 52\r\nContent-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                 *                                   ^
                 *                                   |
                 *                                   tail
                */
                var tail = ByteListUtil.StrStr(bufferStreamUTF8_, "\r\n");
                if (tail == -1)
                {
                    return false;
                }

                {
                    
                    int numericalLen = tail - HeaderContentLengthLength_;
                    var byteArray = bufferStreamUTF8_.GetRange(HeaderContentLengthLength_, numericalLen);
                    var unicodeLen = Encoding.UTF8.GetString(byteArray.ToArray());
                    var value = string.Join("", unicodeLen);
                    contentLength = int.Parse(value);
                }

                /*(Ex)
                (Before) bufferStreamUTF8=Content-Length: 52\nContent-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                (After)  bufferStreamUTF8=Content-Type: application/vscode-jsonrpc; charset=utf8\r\n\r\n{"method":"initialized",...
                */
                bufferStreamUTF8_.RemoveRange(0, tail + 2);//2="\r\n"
            }
            return true;
        }

        bool OnSkipContextHeader()
        {
            lock (bufferStreamUTF8_)
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
                var separatorFirstIndex = ByteListUtil.StrStr(bufferStreamUTF8_, "\r\n");
				if (separatorFirstIndex == -1)
				{
                    return false;
				}
                var separatorEndIndex =ByteListUtil.StrSpn(bufferStreamUTF8_, separatorFirstIndex, "\r\n");
				if (separatorEndIndex == -1)
				{
                    return false;
				}                
                bufferStreamUTF8_.RemoveRange(0, separatorEndIndex);                
                Debug.Assert(bufferStreamUTF8_[0]== Convert.ToByte('{'));
                /*(After) bufferStreamUTF8={"method":"initialized",...
                 */
            }
            return true;
        }
        bool OnParseContext()
        {
            JObject receiver = null;

            lock (bufferStreamUTF8_)
            {
                System.Diagnostics.Debug.Assert(0 < contentLength);
                if (bufferStreamUTF8_.Count < contentLength)
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
                var jsonDataUTF8 = bufferStreamUTF8_.GetRange(0, contentLength);
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
                bufferStreamUTF8_.RemoveRange(0, contentLength);
            }
            var containMethod=receiver.ContainsKey("method");
            var containId = receiver.ContainsKey("id");
			if (containMethod)
			{
				if (containId)
				{
                    Request(receiver);
                    return true;
                }
				else
				{
                    Notification(receiver);
                    return true;
                }
			}
			else
			{
                if (containId)
				{
                    Response(receiver);
                    return true;
				}
				else
				{
                    Debug.Assert(false);
                    Console.WriteLine("[Error]Unknown receiver.");
                    return false;
                }
            }

            
        }
        void Request(JObject receiver)
		{
            var request = receiver.ToObject<RequestMessage>();
            var requestParams= (JObject)request.@params;
            switch (request.method)
			{
                case "workspace/configuration":
                    param_.OnWorkspaceConfiguration(request.id,requestParams.ToObject<ConfigurationParams>());
                    return;
                case "client/registerCapability":
                    param_.OnClientRegisterCapability(request.id, requestParams.ToObject<RegistrationParams>());
                    return;
                case "window/workDoneProgress/create":
                    param_.OnWindowWorkDoneProgressCreate(request.id, requestParams.ToObject<WorkDoneProgressCreateParams>());
                    return;
                default:
                    Console.WriteLine(string.Format("[Error]Not impliment. method={0}/params={1}",request.method, request.@params));
                    return;
            }
		}
        void Notification(JObject receiver)
		{
            var notification = receiver.ToObject<NotificationMessage>();
            var notificationParams = (JObject)notification.@params;
            switch (notification.method)
            {
                case "window/logMessage":
                    var jVal = (JValue)notificationParams["type"];
                    var jMes = (JValue)notificationParams["message"];
                    var val = (MessageType)Enum.ToObject(typeof(MessageType), jVal.Value);
                    var mes = (string)jMes.Value;
                    this.param_.OnWindowLogMessage(new LogMessageParams { type = val, message = mes });
                    return;
                case "window/showMessage":
                    this.param_.OnWindowShowMessage(notificationParams.ToObject<ShowMessageParams>());
                    return;
                case "$/progress":
                    this.param_.OnProgress(notificationParams.ToObject<ProgressParams>());
                    return;
                default:
                    Console.WriteLine(String.Format("[Error]Not impliment. [{0}]{1}", notification.method, notification.@params));
                    return;
            }
        }
        void Response(JObject receiver)
		{            
            Action<JToken> callback=null;
            {
                var id = receiver["id"].ToObject<int>();
                try
                {
                    lock (responseCallback_)
                    {
                        callback = responseCallback_[id];
                        responseCallback_.Remove(id);
                    }
                }
                catch (Exception)
                {
                    //pass
                }
            }

            var response = receiver.ToObject<ResponseMessage>();
            if (response.error == null)
            {
                if (callback == null)
                {
                    //対応するid無し。無視する。
                    Console.WriteLine(string.Format("[対応するid無し]{0}", receiver));
                }
                else
                {
                    callback((JToken)response.result);
                }
            }
            else
            {
                param_.OnResponseError(response);
            }            
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

            System.Diagnostics.Debug.Assert(bufferStreamUTF8_[0] == Convert.ToByte('{'));

            int index = 0;
            int counter = 0;

            foreach (var c in bufferStreamUTF8_)
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
