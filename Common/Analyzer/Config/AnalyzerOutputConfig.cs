using LogsManager.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerOutputConfig
    {
        public int ID { get; set; }

        public OutputTypes OutputType { get; set; }

        public LogMessageParameters[] IncludedMessageParameters { get; set; }

        public string[] IncludedAnalysisParameters { get; set; }
    }
}
