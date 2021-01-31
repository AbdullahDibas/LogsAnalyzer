using LogsManager.Common.Enums;
using System;
using System.Collections.Generic;

namespace LogsManager.Common.Analyzer
{
    public class AnalyzerResultEventArgs : EventArgs
    {
        public int RuleID { get; set; }

        public LogMessage[] Messages { get; set; }

        public Dictionary<string, string> AnalysisParameters { get; set; }
    }
}
