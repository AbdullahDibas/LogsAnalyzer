using LogsManager.Common.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogsManager.Common
{
    public static class Extensions
    {
        public static LogMessageConfig GetLogMessageConfig(this AnalyzerConfig analyzerConfig, int logMessageID)
        {
           return analyzerConfig?.LogMessages?.FirstOrDefault(lm => lm.ID == logMessageID);
        }

        public static LogMessage Copy(this LogMessage logMessage)
        {
            return new LogMessage
            {
                DateTime = logMessage.DateTime,
                Exception = logMessage.Exception,
                LogLevel = logMessage.LogLevel,
                Message = logMessage.Message,
                Parameters = logMessage.Parameters,
                Tags = logMessage.Tags
            }; 
        }
    }
}
