using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using T = Zfg.Core;

namespace Zfg.Core.Application.Logs
{
    internal class NLogger : T.ILogger
    {
        private const string groupName = "SaaS.Community";

        private LogFormatter _formatter;

        private LogFormatter Formatter
        {
            get
            {
                if (this._formatter == null)
                {
                    this._formatter = new LogFormatter("no sessionId", "empty");
                }
                return this._formatter;
            }
        }

        public void Write(Exception exp, string msg)
        {
            string expMsg = this.GetInnerExceptionMessage(exp);
            if (!string.IsNullOrEmpty(msg))
            {
                expMsg = msg + Environment.NewLine + expMsg;
            }
            this.Formatter.Append(expMsg);
            Logger logger = LogManager.GetLogger("SaaS.Community");
            logger.Error(expMsg);
        }

        public void Write(string message)
        {
            this.Formatter.Append(message);
        }

        public void Persistence()
        {
            if (this._formatter != null && this.Formatter.Any)
            {
                StringBuilder text = new StringBuilder();
                Dictionary<string, string> content = this.Formatter.GetContent(
                 string.Empty,
                  string.Empty,
                  groupName);
                foreach (KeyValuePair<string, string> item in content)
                {
                    text.AppendLine(string.Format("{0}：{1}", item.Key, item.Value));
                }
                this.Formatter.Reset();
                Logger logger = LogManager.GetLogger(groupName);
                logger.Info(text.ToString());

            }
        }

        private string GetInnerExceptionMessage(Exception exp)
        {
            StringBuilder sb = new StringBuilder();
            this.ReadExceptionMessage(ref sb, exp);
            return sb.ToString();
        }

        private void ReadExceptionMessage(ref StringBuilder sb, Exception e)
        {
            sb.AppendLine(e.Message);
            sb.AppendLine(e.StackTrace);
            if (e.InnerException != null)
            {
                this.ReadExceptionMessage(ref sb, e.InnerException);
            }
        }

        ///<summary>
        /// 非密封类修饰用protected virtual
        /// 密封类修饰用private
        ///</summary>
        ///<param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                Persistence();
            }

            disposed = true;
        }

        private bool disposed = false;

        ///<summary>
        /// 实现IDisposable中的Dispose方法
        ///</summary>
        public void Dispose()
        {
            //必须为true
            Dispose(true);
        }
    }


    public class LogFormatter
    {
        private StringBuilder content = new StringBuilder();
        private string sessionId, loginUser;
        public bool Any
        {
            get { return content.Length > 0; }

        }
        public LogFormatter(string sessionId, string userName)
        {
            this.sessionId = sessionId;
            this.loginUser = userName;
        }

        public void Reset()
        {
            content = new StringBuilder();

        }

        public void Append(string message)
        {

            content.AppendLine((DateTime.Now.ToString("yyyy-MM-dd/HH.mm.ss.fff：")));
            content.AppendLine((message));
            content.AppendLine();
        }



        public Dictionary<string, string> GetContent(string requestUrl, string requestMethod, string groupName)
        {
            var result = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(requestUrl))
            {
                result.Add("requrl", requestUrl);
            }
            result.Add("content:", content.ToString());
            return result;
        }

    }
    class RequestDetailModel
    {
        public string requestUri { get; set; }
        public string requestMethod { get; set; }
    }
}
