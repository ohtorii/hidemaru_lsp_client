using LSP.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LSP.Client
{


    class Protocol
    {
        public delegate void WindowLogMessageHandler(LogMessageParams param);
        public delegate void WindowShowMessageHandler(ShowMessageParams param);
        public delegate void ResponseErrorHandler(ResponseMessage param);
        public delegate void WorkspaceConfigurationHandler(int id, ConfigurationParams param);
        public delegate void WorkspaceSemanticTokensRefreshHandler();
        public delegate void ClientRegisterCapabilityHandler(int id, RegistrationParams param);
        public delegate void WindowWorkDoneProgressCreateHandler(int id, WorkDoneProgressCreateParams param);
        public delegate void ProgressHandler(ProgressParams param);
        public delegate void TextDocumentPublishDiagnosticsHandler(PublishDiagnosticsParams param);
        /// <summary>
        /// method: ‘window/logMessage’
        /// </summary>
        public event WindowLogMessageHandler OnWindowLogMessage;
        /// <summary>
        /// method: ‘window/showMessage’
        /// </summary>
        public event WindowShowMessageHandler OnWindowShowMessage;
        /// <summary>
        /// method: 'workspace/configuration'
        /// </summary>
        public event WorkspaceConfigurationHandler OnWorkspaceConfiguration;
        /// <summary>
        /// method: 'workspace/semanticTokens/refresh'
        /// </summary>
        public event WorkspaceSemanticTokensRefreshHandler OnWorkspaceSemanticTokensRefresh;
        /// <summary>
        /// method: 'client/registerCapability'
        /// </summary>
        public event ClientRegisterCapabilityHandler OnClientRegisterCapability;
        public event WindowWorkDoneProgressCreateHandler OnWindowWorkDoneProgressCreate;
        /// <summary>
        /// method: ‘$/progress
        /// </summary>
        public event ProgressHandler OnProgress;
        /// <summary>
        /// method: ‘textDocument/publishDiagnostics'
        /// </summary>
        public event TextDocumentPublishDiagnosticsHandler OnTextDocumentPublishDiagnostics;
        enum Mode
        {
            FindContextLength,
            ParseContextLength,
            SkipSeparator,
            ParseContext,
        }
        Mode mode_ = Mode.FindContextLength;
        Dictionary<RequestId, Action<ResponseMessage>> responseCallback_ = new Dictionary<RequestId, Action<ResponseMessage>>();

        CancellationToken cancelToken_;

        int contentLength = -1;
        const string HeaderContentLength_ = "Content-Length:";
        static int HeaderContentLengthLength_ = HeaderContentLength_.Length;

        List<byte> bufferStreamUTF8_ = new List<byte>();
        ILogger logger_;

        public Protocol(CancellationToken token, ILogger logger)
        {
            this.cancelToken_ = token;
            this.logger_ = logger;
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
        public void StoreResponseCallback(RequestId id, Action<ResponseMessage> callback)
        {
            lock (responseCallback_)
            {
                Debug.Assert(responseCallback_.ContainsKey(id) == false);
                responseCallback_.Add(id, callback);
            }
        }
        public async Task Parse()
        {
            //Todo: EventWaitHandle or Task で書き直す
            try
            {
                const int sleepTime = 10;
                while (cancelToken_.IsCancellationRequested == false)
                {
                    switch (mode_)
                    {
                        case Mode.FindContextLength:
                            if (OnFindContextLength() == false)
                            {
                                await Task.Delay(sleepTime, cancelToken_);
                                break;
                            }
                            mode_ = Mode.ParseContextLength;
                            break;
                        case Mode.ParseContextLength:
                            if (OnParseContextLength() == false)
                            {
                                await Task.Delay(sleepTime, cancelToken_);
                                break;
                            }
                            mode_ = Mode.SkipSeparator;
                            break;
                        case Mode.SkipSeparator:
                            if (OnSkipContextHeader() == false)
                            {
                                await Task.Delay(sleepTime, cancelToken_);
                                break;
                            }
                            mode_ = Mode.ParseContext;
                            break;
                        case Mode.ParseContext:
                            if (OnParseContext() == false)
                            {
                                await Task.Delay(sleepTime, cancelToken_);
                                break;
                            }
                            mode_ = Mode.FindContextLength;
                            Restart();
                            break;
                    }
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                //pass
            }
            catch (Exception e)
            {
                if (logger_.IsDebugEnabled)
                {
                    logger_.Error(e.ToString());
                }
                throw;
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
                var separatorEndIndex = ByteListUtil.StrSpn(bufferStreamUTF8_, separatorFirstIndex, "\r\n");
                if (separatorEndIndex == -1)
                {
                    return false;
                }
                bufferStreamUTF8_.RemoveRange(0, separatorEndIndex);
                Debug.Assert(bufferStreamUTF8_[0] == Convert.ToByte('{'));
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
                    logger_.Fatal(e.ToString());
                    throw;
                }

                /*
                 * (Before) bufferStreamUTF8={"method":"initialized",...}Content-Length: 128\r\n{
                 * (After)  bufferStreamUTF8=Content-Length: 128\r\n{
                 */
                bufferStreamUTF8_.RemoveRange(0, contentLength);
            }
            var containMethod = receiver.ContainsKey("method");
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
                    logger_.Error("[Error]Unknown receiver.");
                    return false;
                }
            }


        }
        void Request(JObject receiver)
        {
            //Todo: switchをやめて動的登録にする
            var request = receiver.ToObject<RequestMessage>();
            var requestParams = (JObject)request.@params;
            switch (request.method)
            {
                case "workspace/configuration":
                    this.OnWorkspaceConfiguration(request.id, requestParams.ToObject<ConfigurationParams>());
                    return;
                case "client/registerCapability":
                    this.OnClientRegisterCapability(request.id, requestParams.ToObject<RegistrationParams>());
                    return;
                case "window/workDoneProgress/create":
                    this.OnWindowWorkDoneProgressCreate(request.id, requestParams.ToObject<WorkDoneProgressCreateParams>());
                    return;
                default:
                    if (logger_.IsWarnEnabled)
                    {
                        logger_.Warn(string.Format("Not impliment. method={0}/params={1}", request.method, request.@params));
                    }
                    return;
            }
        }
        void Notification(JObject receiver)
        {
            //Todo: switchをやめて動的登録にする
            var notification = receiver.ToObject<NotificationMessage>();
            var notificationParams = notification.@params as JObject;
            switch (notification.method)
            {
                case "textDocument/publishDiagnostics":
                    this.OnTextDocumentPublishDiagnostics(notificationParams.ToObject<PublishDiagnosticsParams>());
                    return;
                case "workspace/semanticTokens/refresh":
                    this.OnWorkspaceSemanticTokensRefresh();
                    return;
                case "window/logMessage":
                    var jVal = (JValue)notificationParams["type"];
                    var jMes = (JValue)notificationParams["message"];
                    var val = (MessageType)Enum.ToObject(typeof(MessageType), jVal.Value);
                    var mes = (string)jMes.Value;
                    this.OnWindowLogMessage(new LogMessageParams { type = val, message = mes });
                    return;
                case "window/showMessage":
                    this.OnWindowShowMessage(notificationParams.ToObject<ShowMessageParams>());
                    return;
                case "$/progress":
                    this.OnProgress(notificationParams.ToObject<ProgressParams>());
                    return;
                default:
                    if (logger_.IsWarnEnabled)
                    {
                        logger_.Warn(String.Format("Not impliment. [{0}]{1}", notification.method, notification.@params));
                    }
                    return;
            }
        }
        void Response(JObject receiver)
        {
            Action<ResponseMessage> callback = null;
            {
                var id = new RequestId(receiver["id"].ToObject<int>());
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
            if (callback == null)
            {
                //対応するid無し。無視する。
                if (logger_.IsWarnEnabled)
                {
                    logger_.Warn(string.Format("[対応するid無し]{0}", receiver));
                }
            }
            else
            {
                callback(response);
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
                return StrSpn(buf, 0, bufCharSet);
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
