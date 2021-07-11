using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Tracing;

namespace XeniaMobiConnect.ExLogger
{
    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
    }

    public sealed class NLogger : ITraceWriter,ILog
    {
        #region Private member variables.  
        private static readonly Logger ClassLogger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> LoggingMap = new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>> { { TraceLevel.Info, ClassLogger.Info }, { TraceLevel.Debug, ClassLogger.Debug }, { TraceLevel.Error, ClassLogger.Error }, { TraceLevel.Fatal, ClassLogger.Fatal }, { TraceLevel.Warn, ClassLogger.Warn } });
        #endregion

        #region Private properties.  
        /// <summary>  
        /// Get property for Logger  
        /// </summary>  
        private Dictionary<TraceLevel, Action<string>> Logger
        {
            get { return LoggingMap.Value; }
        }
        #endregion

        #region Public member methods.  
        /// <summary>  
        /// Implementation of TraceWriter to trace the logs.  
        /// </summary>  
        /// <param name="request"></param>  
        /// <param name="category"></param>  
        /// <param name="level"></param>  
        /// <param name="traceAction"></param>  
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if (level != TraceLevel.Off)
            {
                if (traceAction != null && traceAction.Target != null)
                {
                    category = category + Environment.NewLine + "Action Parameters : " + traceAction.Target.ToString();
                }
                var record = new TraceRecord(request, category, level);
                if (traceAction != null) traceAction(record);
                Log(record);
            }
        }
        #endregion

        #region Private member methods.  
        /// <summary>  
        /// Logs info/Error to Log file  
        /// </summary>  
        /// <param name="record"></param>  
        private void Log(TraceRecord record)
        {
            var message = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(record.Message))
                message.Append("").Append(record.Message + Environment.NewLine);

            if (record.Request != null)
            {
                if (record.Request.Method != null)
                    message.Append("Method: " + record.Request.Method + Environment.NewLine);

                if (record.Request.RequestUri != null)
                    message.Append("").Append("URL: " + record.Request.RequestUri + Environment.NewLine);
                if (record.Request.Headers != null && record.Request.Headers.Contains("CompanyId") && record.Request.Headers.GetValues("CompanyId") != null && record.Request.Headers.GetValues("CompanyId").FirstOrDefault() != null)
                    message.Append("").Append("CompanyId: " + record.Request.Headers.GetValues("CompanyId").FirstOrDefault() + Environment.NewLine);
                if (record.Request.Headers != null && record.Request.Headers.Contains("BranchId") && record.Request.Headers.GetValues("BranchId") != null && record.Request.Headers.GetValues("BranchId").FirstOrDefault() != null)
                    message.Append("").Append("BranchId: " + record.Request.Headers.GetValues("BranchId").FirstOrDefault() + Environment.NewLine);
                if (record.Request.Headers != null && record.Request.Headers.Contains("Authorization") && record.Request.Headers.GetValues("Authorization") != null && record.Request.Headers.GetValues("Authorization").FirstOrDefault() != null)
                    message.Append("").Append("Authorization: " + record.Request.Headers.GetValues("Authorization").FirstOrDefault() + Environment.NewLine);
            }

            if (!string.IsNullOrWhiteSpace(record.Category))
                message.Append("").Append(record.Category);

            if (!string.IsNullOrWhiteSpace(record.Operator))
                message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);

            if (record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
            {
                var exceptionType = record.Exception.GetType();
                message.Append(Environment.NewLine);
                message.Append("").Append("Error: " + record.Exception.GetBaseException().Message + Environment.NewLine);
            }

            Logger[record.Level](Convert.ToString(message) + Environment.NewLine);
        }
        #endregion

        public void Information(string message)
        {
            ClassLogger.Info(message);
        }

        public void Warning(string message)
        {
            ClassLogger.Warn(message);
        }

        public void Debug(string message)
        {
            ClassLogger.Debug(message);
        }

        public void Error(string message)
        {
            ClassLogger.Error(message);
        }
    }
}