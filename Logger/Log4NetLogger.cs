using LogsManager.Common;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;

namespace Logger
{
    public class Log4NetLogger : ILogger
    {
        public void WriteErrorLog(LogLevels logLevel, string message, Exception exception = null, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public void WriteLog(LogLevels logLevel, string message, string[] tags = null, params KeyValuePair<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }
    }
}
