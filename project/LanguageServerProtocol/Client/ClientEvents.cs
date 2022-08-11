using LSP.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace LSP.Client
{
    class ClientEvents
    {
        LanguageClient.LspParameter param_;
        Action<int, JArray> sendResponse_;
        Dictionary<string, PublishDiagnosticsParams> publishDiagnostics_ = new Dictionary<string, PublishDiagnosticsParams>();

        public ClientEvents(LanguageClient.LspParameter param, Action<int, JArray> sendResponse)
        {
            param_ = param;
            sendResponse_ = sendResponse;
        }
        public void OnWindowLogMessage(LogMessageParams param)
        {
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace(String.Format("[OnWindowLogMessage]{0}", param.message));
            }
        }
        public void OnWindowShowMessage(ShowMessageParams param)
        {
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace(String.Format("[OnWindowShowMessage]{0}", param.message));
            }
        }
        public void OnWorkspaceConfiguration(int request_id, ConfigurationParams param)
        {
            var any = new JArray();
            foreach (var item in param.items)
            {
                try
                {
                    var jsonValue = param_.jsonWorkspaceConfiguration[item.section];
                    any.Add(jsonValue);
                }
                catch (Exception)
                {
                    any.Add(null);
                }
            }
            //"workspace/configuration" に対する返信。
            sendResponse_(request_id, any);
        }
        public void OnWorkspaceSemanticTokensRefresh()
        {
            //Todo: workspace/semanticTokens/refresh
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace("Todo:workspace/semanticTokens/refresh");
            }
        }
        public void OnClientRegisterCapability(int id, RegistrationParams param)
        {
            //Todo: client/registerCapability
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace("Todo: client/registerCapability");
            }
        }
        public void OnWindowWorkDoneProgressCreate(int id, WorkDoneProgressCreateParams param)
        {
            //Todo: window/workDoneProgress/create
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace("Todo: window/workDoneProgress/create");
            }
        }
        public void OnProgress(ProgressParams param)
        {
            //Todo: $/progress
            if (param_.logger.IsTraceEnabled)
            {
                param_.logger.Trace("Todo: $/progress");
            }
        }

        #region publishDiagnostics
        static string makePublishDiagnosticsKey(string uri)
        {
            return Util.NormalizeUri(uri).ToLower();
        }
        public void OnTextDocumentPublishDiagnostics(PublishDiagnosticsParams param)
        {
            if ((param.diagnostics == null) || (param.diagnostics.Length == 0))
            {
                return;
            }
            lock (publishDiagnostics_)
            {
                publishDiagnostics_[makePublishDiagnosticsKey(param.uri)] = param;
            }
        }
        /// <summary>
        /// ‘textDocument/publishDiagnostics’通知を取得する
        /// </summary>
        /// <returns>対応するドキュメントURIが存在していればnull以外、存在しなければnull</returns>
        public PublishDiagnosticsParams[] PullTextDocumentPublishDiagnostics()
        {
            PublishDiagnosticsParams[] result;
            int index = 0;
            lock (publishDiagnostics_)
            {
                result = new PublishDiagnosticsParams[publishDiagnostics_.Count];
                foreach (var item in publishDiagnostics_)
                {
                    result[index] = item.Value;
                    ++index;
                }
                publishDiagnostics_.Clear();
            }
            return result;
        }
        #endregion
    }
}
