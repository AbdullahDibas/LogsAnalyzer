using LogsManager.Common.Analyzer.Config;
using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer
{
    public class LogMessageConfig
    {
        public int ID { get; set; }

        public LogMessageParameter[] LogMessageParameters { get; set; }
    }
}
