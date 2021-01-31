using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;

namespace LogsManager.Common
{
    public interface ILogsAnalyzer
    {
        void AnalyzeLog(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters);
       
        void AnalyzeLog(LogMessage logMessage);

        void Dispose();
    }
}
