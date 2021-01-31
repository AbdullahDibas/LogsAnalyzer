using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;

namespace LogsManager.Common
{
    public class LogMessage
    {
        public DateTime DateTime { get; set; }

        public string Message { get; set; }

        public LogLevels LogLevel { get; set; }

        public string[] Tags { get; set; }

        public KeyValuePair<string, string>[] Parameters { get; set; }

        public Exception Exception { get; set; }
    }
}
