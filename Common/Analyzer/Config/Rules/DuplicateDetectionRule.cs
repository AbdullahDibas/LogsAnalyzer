using System;
using System.Collections.Generic;
using System.Text;

namespace LogsManager.Common.Analyzer.Rules
{
    public class DuplicateDetectionRule : AnalyzerRuleConfig
    {
        public int LogMessageID { get; set; }

        public int MaximumNumberOfMonitoredMessages { get; set; }
    }
}
