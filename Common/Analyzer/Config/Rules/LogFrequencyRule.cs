using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class LogFrequencyRule : AnalyzerRuleConfig
    {
        public int LogMessageID { get; set; }
        public int TimeInSeconds { get; set; }
    }
}
